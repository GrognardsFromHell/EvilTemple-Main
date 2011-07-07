#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using EvilTemple.Runtime;

#endregion

namespace EvilTemple.NativeEngineInterop
{

    public class UserInterface
    {
        private readonly IntPtr _handle;

        private readonly DynamicQObjectWrapper _rootWidget;

        public event Action<int, object> CreateObjectSuccess
        {
            add { ConnectionManager.Connect(_rootWidget.Handle, "createObjectSuccess", value); }
            remove { ConnectionManager.Disconnect(_rootWidget.Handle, "createObjectSuccess", value); }
        }

        public event Action<int, string> CreateObjectError
        {
            add { ConnectionManager.Connect(_rootWidget.Handle, "createObjectError", value); }
            remove { ConnectionManager.Disconnect(_rootWidget.Handle, "createObjectError", value); }
        }

        internal UserInterface(IntPtr handle)
        {
            _handle = handle;
            _rootWidget = new DynamicQObjectWrapper(_handle);
        }

        /// <summary>
        ///   The root interface item.
        /// </summary>
        public dynamic RootInterfaceItem
        {
            get { return _rootWidget; }
        }

        public static readonly Action<string> DefaultErrorHandler = error => Console.WriteLine(error);

        public void AddWidget(string url, Action<object> success = null, Action<string> error = null)
        {
            if (error == null)
                error = DefaultErrorHandler;

            Action<int, object>[] successCallback = {null}; // Array hack for closure
            Action<int, string>[] errorCallback = {null};
            
            successCallback[0] = (ticket, result) =>
            {
                CreateObjectSuccess -= successCallback[0];
                CreateObjectError -= errorCallback[0];

                if (success != null)
                    success(result);
            };

            errorCallback[0] = (ticket, result) =>
            {
                CreateObjectSuccess -= successCallback[0];
                CreateObjectError -= errorCallback[0];

                error(result);
            };

            CreateObjectSuccess += successCallback[0];
            CreateObjectError += errorCallback[0];

            RootInterfaceItem.createObject(url);
        }

        private const string PROXY_SUFFIX = "Proxy";
        private const string ASSEMBLY_NAME = "ProxyAssembly";
        private const string MODULE_NAME = "ProxyModule";
        private const string HANDLE_NAME = "handle";

        /// <summary>
        /// Adds a widget item and uses the given interface to encapsulate the widget.
        /// </summary>
        public void AddWidget<T>(string url, Action<T> successCallback = null, Action<string> errorCallback = null) where T : class
        {
            AddWidget(url, obj => ProcessCreateObjectResult(obj, successCallback, errorCallback), errorCallback);
        }

        private static void ProcessCreateObjectResult<T>(object result, Action<T> successCallback, Action<string> errorCallback) where T : class
        {
            if (result is string)
                throw new ArgumentException(result.ToString());

            if (!(result is DynamicQObjectWrapper))
                throw new ArgumentException("Cannot cast non QObject to required type. Got: " + result.GetType());

            var handle = ((DynamicQObjectWrapper)result).Handle;

            var domain = Thread.GetDomain();
            var assemblyName = new AssemblyName
                                   {
                                       Name = ASSEMBLY_NAME,
                                       Version = new Version(1, 0, 0, 0)
                                   };

            var assemblyBuilder = domain.DefineDynamicAssembly(assemblyName,
                                                               AssemblyBuilderAccess.Run);

            // create a new module for this proxy
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(MODULE_NAME);

            // Set the class to be public and sealed
            var typeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed;

            var objType = typeof(object);
            var interfaces = new[] { typeof(T) };
            var typeName = typeof(T).Name + "Wrapper";

            var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, objType, interfaces);

            // Define a member variable to hold the delegate
            var handleField = typeBuilder.DefineField(HANDLE_NAME, typeof(IntPtr), FieldAttributes.Private);

            // Build a constructor taking the handle
            BuildConstructor(typeBuilder, handleField);

            // Implement interface properties
            ImplementProperties(typeBuilder, typeof(T), handleField);

            // Implement interface methods
            ImplementMethods(typeBuilder, typeof(T), handleField);

            // Implement interface events
            ImplementSignals(typeBuilder, typeof(T), handleField);

            var type = typeBuilder.CreateType();

            successCallback((T) Activator.CreateInstance(type, handle));
        }

        private static void ImplementProperties(TypeBuilder typeBuilder, Type interfaceType, FieldInfo handleField)
        {
            var interfaceProperties = interfaceType.GetProperties();

            foreach (var propertyInfo in interfaceProperties)
                ImplementProperty(typeBuilder, propertyInfo, handleField);
        }

        private static void ImplementProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldInfo handleField)
        {

            var setMethod = typeBuilder.Implement(propertyInfo.GetSetMethod());
            EmitPropertySetter(propertyInfo, setMethod.GetILGenerator(), handleField);

            var getMethod = typeBuilder.Implement(propertyInfo.GetGetMethod());
            EmitPropertyGetter(propertyInfo, getMethod.GetILGenerator(), handleField);

            var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, CallingConventions.Standard,
                                       propertyInfo.PropertyType,
                                       new Type[0]);
            propertyBuilder.SetSetMethod(setMethod);
            propertyBuilder.SetGetMethod(getMethod);
        }

        private static void EmitPropertySetter(PropertyInfo propertyInfo, ILGenerator generator, FieldInfo handleField)
        {
            var propertyNativeName = GetNativeMethodName(propertyInfo.Name);
            var propertyValue = generator.DeclareLocal(typeof(IntPtr));

            // Prepare call to QObject_SetProperty
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField); // Read and push QObject handle
            generator.Emit(OpCodes.Ldstr, propertyNativeName);

            // Convert the new property value to a QVariant pointer and leave it on the stack
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, SelectConversionMethod(propertyInfo.PropertyType));
            generator.Emit(OpCodes.Stloc, propertyValue);
            generator.Emit(OpCodes.Ldloc, propertyValue);

            generator.Emit(OpCodes.Call, SetPropertyMethod);

            // Free the variant pointer
            generator.Emit(OpCodes.Ldloc, propertyValue);
            generator.Emit(OpCodes.Call, FreeMethod);

            // Interpret result code (probably either invalid type *or* property not found)
            var errorLabel = generator.DefineLabel();
            generator.Emit(OpCodes.Brfalse, errorLabel);
            generator.Emit(OpCodes.Ret);

            // Call error handler
            generator.MarkLabel(errorLabel);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField);
            generator.Emit(OpCodes.Ldstr, propertyNativeName);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, RaisePropertySetErrorMethod);

            generator.Emit(OpCodes.Ret);
        }

        private static void EmitPropertyGetter(PropertyInfo propertyInfo, ILGenerator generator, FieldInfo handleField)
        {
            var propertyNativeName = GetNativeMethodName(propertyInfo.Name);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField);
            generator.Emit(OpCodes.Ldstr, propertyNativeName);
            generator.Emit(OpCodes.Ldtoken, propertyInfo.PropertyType);
            generator.Emit(OpCodes.Call, GetPropertyMethod);
            generator.Emit(OpCodes.Ret);
        }

        public static void RaisePropertySetError(IntPtr qObjectHandle, string propertyName, object value)
        {
            var message = string.Format("Unable to set property {0} on QObject of type {1}. Value={2}", propertyName,
                                        DynamicQObjectWrapper.QObject_GetClassName(qObjectHandle), value);
            throw new InvalidOperationException(message);
        }

        public static object GetProperty(IntPtr qObjectHandle, string propertyName, Type requestedType)
        {
            var result = DynamicQObjectWrapper.QObject_GetProperty(qObjectHandle, propertyName);
            if (result == IntPtr.Zero)
                throw new InvalidOperationException("Unable to get property " + propertyName + " of QObject type " +
                          DynamicQObjectWrapper.QObject_GetClassName(qObjectHandle));

            try
            {
                return VariantHelper.ToObject(result);
            }
            finally
            {
                VariantHelper.Free(result);
            }
        }

        private static void ImplementSignals(TypeBuilder typeBuilder, Type interfaceType, FieldBuilder handleField)
        {
            var interfaceEvents = interfaceType.GetEvents();

            foreach (var eventInfo in interfaceEvents)
            {
                ImplementEvent(typeBuilder, eventInfo, handleField);
            }
        }

        private static void ImplementEvent(TypeBuilder typeBuilder, EventInfo eventInfo, FieldInfo handleField)
        {
            /*
             * Build the Add method
             */
            var addMethod = typeBuilder.Implement(eventInfo.GetAddMethod());
            EmitConnectOrDisconnect(eventInfo, addMethod, handleField, ConnectMethod);

            /*
             * Build the remove method
             */
            var removeMethod = typeBuilder.Implement(eventInfo.GetRemoveMethod());
            EmitConnectOrDisconnect(eventInfo, removeMethod, handleField, DisconnectMethod);

            var eventBuilder = typeBuilder.DefineEvent(eventInfo.Name, EventAttributes.None, eventInfo.EventHandlerType);
            eventBuilder.SetAddOnMethod(addMethod);
            eventBuilder.SetRemoveOnMethod(removeMethod);
        }

        private static void EmitConnectOrDisconnect(EventInfo eventInfo, MethodBuilder method, FieldInfo handleField, MethodInfo connectMethod)
        {
            var generator = method.GetILGenerator();

            // arg 1: Load the handle onto the stack
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField);

            // arg 2: The signal to connect to
            generator.Emit(OpCodes.Ldstr, GetNativeMethodName(eventInfo.Name));

            // arg 3: Load the delegate onto the stack
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, connectMethod);

            generator.Emit(OpCodes.Ret);
        }

        private static void ImplementMethods(TypeBuilder typeBuilder, Type interfaceType, FieldInfo handleField)
        {
            var interfaceMethods = interfaceType.GetMethods();

            foreach (var methodInfo in interfaceMethods)
                ImplementMethod(methodInfo, typeBuilder, handleField);
        }

        private static void ImplementMethod(MethodInfo methodInfo, TypeBuilder typeBuilder, FieldInfo handleField)
        {
            // Properties and events are handled elsewhere
            if (methodInfo.IsSpecialName)
                return;

            // Get the method parameters since we need to create an array
            // of parameter types                         
            var methodParams = methodInfo.GetParameters();
            var numOfParams = methodParams.Length;
            var methodParameters = new Type[numOfParams];

            // convert the ParameterInfo objects into Type
            for (var j = 0; j < numOfParams; j++)
            {
                // Check that the parameters can actually be converted
                methodParameters[j] = methodParams[j].ParameterType;
            }

            // create a new builder for the method in the interface
            var methodBuilder = typeBuilder.DefineMethod(
                methodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                CallingConventions.Standard,
                methodInfo.ReturnType,
                methodParameters);

            var generator = methodBuilder.GetILGenerator();

            // Emit a declaration of a local variable if there is a return
            // type defined
            var resultBuilder = generator.DeclareLocal(typeof(IntPtr));
            var successBuilder = generator.DeclareLocal(typeof(bool));

            // if we have any parameters for the method, then declare an 
            // Object array local var.
            LocalBuilder argsBuilder = null;
            if (numOfParams > 0)
            {
                argsBuilder = generator.DeclareLocal(typeof(IntPtr[]));
                generator.Emit(OpCodes.Ldc_I4, numOfParams);
                generator.Emit(OpCodes.Newarr, typeof(IntPtr));
                generator.Emit(OpCodes.Stloc, argsBuilder);

                for (var i = 0; i < numOfParams; ++i)
                {
                    generator.Emit(OpCodes.Ldloc, argsBuilder);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Ldelema, typeof(IntPtr));

                    // Call conversion method for argument i
                    generator.Emit(OpCodes.Ldarg, 1 + i);
                    var conversionMethod = SelectConversionMethod(methodParameters[i]);
                    generator.Emit(OpCodes.Call, conversionMethod);

                    // Store result of conversion @ loaded address
                    generator.Emit(OpCodes.Stobj, typeof(IntPtr));
                }
            }

            // Prepare to call the try invoke member function
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField);
            generator.Emit(OpCodes.Ldstr, GetNativeMethodName(methodInfo.Name));
            if (argsBuilder != null)
                generator.Emit(OpCodes.Ldloc, argsBuilder.LocalIndex);
            else
                generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ldc_I4, numOfParams);
            generator.Emit(OpCodes.Ldloca, resultBuilder);

            generator.Emit(OpCodes.Call, CallMetaMethodMethod);
            generator.Emit(OpCodes.Stloc, successBuilder);

            // Free all argument pointers
            if (argsBuilder != null)
            {
                for (var i = 0; i < numOfParams; ++i)
                {
                    generator.Emit(OpCodes.Ldloc, argsBuilder);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Ldelem, typeof(IntPtr));
                    generator.Emit(OpCodes.Call, FreeMethod);
                }
            }

            var hasReturnValue = !methodInfo.ReturnType.Equals(typeof(void));

            if (!hasReturnValue)
            {
                // Make sure the return value is released
                generator.Emit(OpCodes.Ldloc, resultBuilder);
                generator.Emit(OpCodes.Call, FreeResultMethod);
            }
            else
            {
                // Convert and box/unbox the result value, then free it
            }

            var errorLabel = generator.DefineLabel();

            // Interpret the success
            generator.Emit(OpCodes.Ldloc, successBuilder);
            generator.Emit(OpCodes.Brfalse, errorLabel);
            generator.Emit(OpCodes.Ret);

            generator.MarkLabel(errorLabel);

            /*
             * Build argument list and call the error function.
             */
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, handleField);
            generator.Emit(OpCodes.Ldstr, GetNativeMethodName(methodInfo.Name));

            for (var i = 0; i < numOfParams; ++i)
                generator.Emit(OpCodes.Ldarg, 1 + i);

            generator.EmitCall(OpCodes.Call, RaiseCallErrorMethod, methodParameters);

            generator.Emit(OpCodes.Ret);
        }

        private static string GetNativeMethodName(string name)
        {
            var tmp = name.ToCharArray();
            tmp[0] = char.ToLower(tmp[0]);
            return new string(tmp);
        }

        /// <summary>
        /// Selects a conversion method from native datatype to a QVariant pointer.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static MethodInfo SelectConversionMethod(Type type)
        {
            if (typeof(double).IsAssignableFrom(type))
                return ConvertDoubleMethod;
            if (typeof(float).IsAssignableFrom(type))
                return ConvertFloatMethod;
            if (typeof(ulong).IsAssignableFrom(type))
                return ConvertULongMethod;
            if (typeof(long).IsAssignableFrom(type))
                return ConvertLongMethod;
            if (typeof(int).IsAssignableFrom(type))
                return ConvertIntMethod;
            if (typeof(uint).IsAssignableFrom(type))
                return ConvertUIntMethod;
            if (typeof(short).IsAssignableFrom(type))
                return ConvertShortMethod;
            if (typeof(ushort).IsAssignableFrom(type))
                return ConvertUShortMethod;
            if (typeof(byte).IsAssignableFrom(type))
                return ConvertByteMethod;
            if (typeof(sbyte).IsAssignableFrom(type))
                return ConvertSByteMethod;
            if (typeof(string).IsAssignableFrom(type))
                return ConvertStringMethod;
            if (typeof(bool).IsAssignableFrom(type))
                return ConvertBoolMethod;
            // TODO: 
            if (type.IsArray)
                return ConvertByteMethod;

            throw new ArgumentException("Unsupported type " + type + " in method signature.");
        }

        /// <summary>
        /// Adds a constructor to the given type builder. The constructor takes a single argument,
        /// the native handle to the underlying object, and puts it into the private handle field.
        /// </summary>
        /// <param name="typeBuilder">The builder for the dynamic implementation.</param>
        /// <param name="handlerField">The field in which the handle will be stored.</param>
        private static void BuildConstructor(TypeBuilder typeBuilder, FieldInfo handlerField)
        {
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(IntPtr) });

            var generator = constructor.GetILGenerator();

            // Store arg1 in handler field of arg0 (this)
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, handlerField);

            // Remember to call the base class constructor
            var baseConstructor = typeof(object).GetConstructor(new Type[0]);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, baseConstructor);

            // Return from the constructor
            generator.Emit(OpCodes.Ret);
        }

        #region ConversionMethodInfos

        private static readonly MethodInfo ConnectMethod = typeof(ConnectionManager).GetMethod("Connect");
        private static readonly MethodInfo DisconnectMethod = typeof(ConnectionManager).GetMethod("Disconnect");
        private static readonly MethodInfo CallMetaMethodMethod = typeof(DynamicQObjectWrapper).GetMethod("QObject_CallMetaMethod");
        private static readonly MethodInfo SetPropertyMethod = typeof(DynamicQObjectWrapper).GetMethod("QObject_SetProperty");
        private static readonly MethodInfo FreeMethod = typeof(VariantHelper).GetMethod("Free", new[] { typeof(IntPtr) });
        private static readonly MethodInfo FreeResultMethod = typeof(UserInterface).GetMethod("FreeResult", new[] { typeof(IntPtr) });
        private static readonly MethodInfo RaiseCallErrorMethod = typeof(UserInterface).GetMethod("RaiseCallError");
        private static readonly MethodInfo RaisePropertySetErrorMethod = typeof(UserInterface).GetMethod("RaisePropertySetError");
        private static readonly MethodInfo GetPropertyMethod = typeof(UserInterface).GetMethod("GetProperty");
        private static readonly MethodInfo ConvertBoolMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(bool) });
        private static readonly MethodInfo ConvertStringMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(string) });
        private static readonly MethodInfo ConvertDoubleMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(double) });
        private static readonly MethodInfo ConvertFloatMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(float) });
        private static readonly MethodInfo ConvertLongMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(long) });
        private static readonly MethodInfo ConvertULongMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(ulong) });
        private static readonly MethodInfo ConvertIntMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(int) });
        private static readonly MethodInfo ConvertUIntMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(uint) });
        private static readonly MethodInfo ConvertShortMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(short) });
        private static readonly MethodInfo ConvertUShortMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(ushort) });
        private static readonly MethodInfo ConvertByteMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(byte) });
        private static readonly MethodInfo ConvertSByteMethod = typeof(VariantHelper).GetMethod("ToVariant", new[] { typeof(sbyte) });
        #endregion

        #region PInvoke

        // ReSharper disable InconsistentNaming
        [DllImport("NativeEngine")]
        private static extern IntPtr UserInterface_GetRootWidget(IntPtr handle);

        // ReSharper restore InconsistentNaming

        #endregion

        private IntPtr _myhandle;

        public static void FreeResult(IntPtr result)
        {
            if (result != IntPtr.Zero)
                VariantHelper.Free(result);
        }

        public static void RaiseCallError(IntPtr handle, string methodName, params object[] args)
        {
            var className = DynamicQObjectWrapper.QObject_GetClassName(handle);

            var message = string.Format("Unable to find method {0} in QObject class {1} with arguments: {2}",
                                        methodName, className, args);

            throw new MissingMethodException(message, methodName);
        }
    }

    public class DynamicQObjectWrapper : DynamicObject
    {
        internal readonly IntPtr Handle;

        public DynamicQObjectWrapper(IntPtr handle)
        {
            Handle = handle;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var genericArgs = new IntPtr[args.Length];
            IntPtr variantResult;
            IntPtr errorMessage;
            result = null;

            for (var i = 0; i < args.Length; ++i)
                genericArgs[i] = VariantHelper.ToVariant(args[i]);

            try
            {
                if (!QObject_CallMetaMethod(Handle, binder.Name, genericArgs, genericArgs.Length, out variantResult, out errorMessage))
                {
                    /* The Call Meta Method function may throw an "exception" by setting the error string, we have to handle this here */
                    if (errorMessage != IntPtr.Zero)
                    {
                        var errorMessageText = QStringHelper.GetString(errorMessage);
                        QStringHelper.Free(errorMessage);
                        throw new InvalidOperationException(errorMessageText);
                    }

                    return false;
                }

                Trace.Assert(errorMessage == IntPtr.Zero,
                             "If calling a meta method succeeds, the error message must be NULL.");
            }
            finally
            {
                for (var i = 0; i < args.Length; ++i)
                    VariantHelper.Free(genericArgs[i]);
            }

            // Convert generic result to result
            result = VariantHelper.ToObject(variantResult);

            return true;
        }

        public void Connect(string signal, Delegate callback)
        {
            ConnectionManager.Connect(Handle, signal, callback);
        }

        #region PInvoke

        // ReSharper disable InconsistentNaming
        [DllImport("NativeEngine")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool QObject_CallMetaMethod(IntPtr handle, string name, IntPtr[] args, int argsCount, out IntPtr result, out IntPtr errorMessage);

        [DllImport("NativeEngine")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool QObject_SetProperty(IntPtr handle, string name, IntPtr value);

        [DllImport("NativeEngine")]
        public static extern IntPtr QObject_GetProperty(IntPtr handle, string name);

        [DllImport("NativeEngine")]
        internal static extern string QObject_GetClassName(IntPtr handle);
        // ReSharper restore InconsistentNaming

        #endregion
    }

    public static class VariantHelper
    {
        public static object ToObject(IntPtr variant)
        {
            if (variant == IntPtr.Zero)
                return null;

            var type = Variant_GetType(variant);

            switch (type)
            {
                case "QObject*":
                    var qObjectStar = Variant_GetQObjectStar(variant);
                    return new DynamicQObjectWrapper(qObjectStar);
                case "bool":
                    return Variant_ToBool(variant);
                case "int":
                    return Variant_ToInt(variant);
                case "double":
                    return Variant_ToReal(variant);
                case "QColor":
                case "QString":
                    {
                        var handle = Variant_ToString(variant);
                        var result = QStringHelper.GetString(handle);
                        QStringHelper.Free(handle);
                        return result;
                    }
                case "QVariantList":
                    return ToObjectFromVariantList(variant);
                case "QVariantMap":
                    return ToObjectFromVariantMap(variant);
                default:
                    // Get opaque handle?
                    throw new ArgumentException("Unknown variant type: " + type);
            }
        }

        private static object[] ToObjectFromVariantList(IntPtr variant)
        {
            var variantList = Variant_ToList(variant);

            var result = new object[VariantListHelper.Length(variantList)];

            for (var i = 0; i < result.Length; ++i)
            {
                var element = VariantListHelper.Get(variantList, i);
                result[i] = ToObject(element);
                Free(element);
            }

            VariantListHelper.Free(variantList);

            return result;
        }

        private static Dictionary<string, object> ToObjectFromVariantMap(IntPtr variant)
        {
            var variantMap = Variant_ToMap(variant);

            var capacity = VariantMapHelper.Size(variantMap);
            var result = new Dictionary<string, object>(capacity);

            var keys = VariantMapHelper.Keys(variantMap);
            var keyCount = StringListHelper.Length(keys);

            for (var i = 0; i < keyCount; ++i)
            {
                var keyHandle = StringListHelper.Get(keys, i);
                var key = QStringHelper.GetString(keyHandle);

                var element = VariantMapHelper.Get(variantMap, key);
                result.Add(key, ToObject(element));
                Free(element);

                QStringHelper.Free(keyHandle);
            }

            VariantMapHelper.Free(variantMap);

            return result;
        }

        public static IntPtr ToVariant(object obj)
        {
            if (obj == null)
                return Variant_NewNull();

            if (obj is string)
                return Variant_FromString((string)obj);

            if (obj is int)
                return Variant_FromInt((int)obj);

            if (obj is float)
                return Variant_FromReal((float)obj);

            if (obj is bool)
                return Variant_FromBool((bool)obj);

            throw new ArgumentException("Cannot convert " + obj.GetType() + " to generic argument.");
        }

        public static IntPtr ToVariant(bool value)
        {
            return Variant_FromBool(value);
        }

        public static IntPtr ToVariant(string value)
        {
            return Variant_FromString(value);
        }

        public static IntPtr ToVariant(double number)
        {
            return Variant_FromReal(number);
        }

        public static IntPtr ToVariant(float number)
        {
            return Variant_FromReal(number);
        }

        public static IntPtr ToVariant(long number)
        {
            return Variant_FromReal(number);
        }

        public static IntPtr ToVariant(ulong number)
        {
            return Variant_FromReal(number);
        }

        public static IntPtr ToVariant(int number)
        {
            return Variant_FromInt(number);
        }

        public static IntPtr ToVariant(uint number)
        {
            return Variant_FromInt((int)number);
        }

        public static IntPtr ToVariant(short number)
        {
            return Variant_FromInt(number);
        }

        public static IntPtr ToVariant(ushort number)
        {
            return Variant_FromInt(number);
        }

        public static IntPtr ToVariant(byte number)
        {
            return Variant_FromInt(number);
        }

        public static IntPtr ToVariant(sbyte number)
        {
            return Variant_FromInt(number);
        }

        #region PInvoke

        [DllImport("NativeEngine", CharSet = CharSet.Unicode)]
        private static extern IntPtr Variant_FromString(string s);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_FromInt(int n);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_FromReal(double n);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_FromBool(bool n);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_NewNull();

        [DllImport("NativeEngine")]
        private static extern string Variant_GetType(IntPtr handle);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_GetQObjectStar(IntPtr handle);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_ToString(IntPtr variant);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_ToList(IntPtr variant);

        [DllImport("NativeEngine")]
        private static extern IntPtr Variant_ToMap(IntPtr variant);

        [DllImport("NativeEngine")]
        private static extern int Variant_ToInt(IntPtr variant);

        [DllImport("NativeEngine")]
        private static extern double Variant_ToReal(IntPtr variant);

        [DllImport("NativeEngine")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Variant_ToBool(IntPtr variant);

        [DllImport("NativeEngine", EntryPoint = "Variant_Free")]
        public static extern void Free(IntPtr variant);

        #endregion
    }

    internal static class VariantListHelper
    {
        [DllImport("NativeEngine", EntryPoint = "VariantList_New")]
        public static extern IntPtr New(int capacity);

        [DllImport("NativeEngine", EntryPoint = "VariantList_Length")]
        public static extern int Length(IntPtr handle);

        [DllImport("NativeEngine", EntryPoint = "VariantList_Get")]
        public static extern IntPtr Get(IntPtr handle, int index);

        [DllImport("NativeEngine", EntryPoint = "VariantList_Add")]
        public static extern void Add(IntPtr handle, IntPtr variant);

        [DllImport("NativeEngine", EntryPoint = "VariantList_Free")]
        public static extern void Free(IntPtr handle);
    }
    
    internal static class VariantMapHelper
    {
        [DllImport("NativeEngine", EntryPoint = "VariantMap_New")]
        public static extern IntPtr New();

        [DllImport("NativeEngine", EntryPoint = "VariantMap_Size")]
        public static extern int Size(IntPtr handle);

        [DllImport("NativeEngine", EntryPoint = "VariantMap_Keys")]
        public static extern IntPtr Keys(IntPtr handle);

        [DllImport("NativeEngine", EntryPoint = "VariantMap_Get", CharSet = CharSet.Unicode)]
        public static extern IntPtr Get(IntPtr handle, string key);

        [DllImport("NativeEngine", EntryPoint = "VariantMap_Add", CharSet = CharSet.Unicode)]
        public static extern void Add(IntPtr handle, string key, IntPtr value);

        [DllImport("NativeEngine", EntryPoint = "VariantMap_Free")]
        public static extern void Free(IntPtr handle);
    }

    internal static class StringListHelper
    {
        [DllImport("NativeEngine", EntryPoint = "StringList_Length")]
        public static extern int Length(IntPtr handle);

        [DllImport("NativeEngine", EntryPoint = "StringList_Get")]
        public static extern IntPtr Get(IntPtr handle, int i);

        [DllImport("NativeEngine", EntryPoint = "StringList_Free")]
        public static extern void Free(IntPtr handle);
    }

    internal static class QStringHelper
    {
        [DllImport("NativeEngine", CharSet = CharSet.Unicode, EntryPoint = "QString_Utf16")]
        public static extern string GetString(IntPtr handle);

        [DllImport("NativeEngine", EntryPoint = "QString_Free")]
        public static extern void Free(IntPtr handle);
    }

    public static class ConnectionManager
    {
        private struct Connection
        {
            public Delegate Callback;
            public IntPtr Sender;
            public string Signal;
        }

        /// <summary>
        /// Holds the delegates that are connected to a given slot id.
        /// </summary>
        private static readonly Dictionary<int, Connection> Connections = new Dictionary<int, Connection>();

        /// <summary>
        /// This is the C# version of the following function pointer typedef:
        /// typedef void (__stdcall *CallSlotFn)(int slotId, int paramCount, const char **paramTypes, void **paramValues);
        /// </summary>
        /// <param name="slotId">The id of the slot that was called.</param>
        /// <param name="paramCount">The number of parameters. Used to initialize paramTypes and paramValues.</param>
        /// <param name="paramTypes">The types of the parameters to this slot.</param>
        /// <param name="paramValues">Pointers to the actual arguments.</param>
        private delegate void CallSlotDelegate(int slotId,
            int paramCount,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)] string[] paramTypes,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IntPtr[] paramValues);

        /// <summary>
        /// This is the C# equivalent of the following function pointer typedef:
        /// typedef void (__stdcall *DisconnectCallback)(int slotId);
        /// </summary>
        /// <param name="slotId"></param>
        private delegate void SlotDisconnectedDelegate(int slotId);

        private static readonly CallSlotDelegate CallSlotHolder;

        private static readonly SlotDisconnectedDelegate SlotDisconnectedHolder;

        static ConnectionManager()
        {
            CallSlotHolder = new CallSlotDelegate(SlotCalled);
            SlotDisconnectedHolder = new SlotDisconnectedDelegate(SlotDisconnected);

            ConnectionManager_SetCallSlotCallback(Marshal.GetFunctionPointerForDelegate(CallSlotHolder));
            ConnectionManager_SetDisconnectCallback(Marshal.GetFunctionPointerForDelegate(SlotDisconnectedHolder));
        }

        private static void SlotCalled(int slotId, int paramCount, string[] paramTypes, IntPtr[] paramValues)
        {
            Console.WriteLine("Slot called: " + slotId);

            Connection connection;
            if (!Connections.TryGetValue(slotId, out connection))
            {
                Trace.TraceError("Slot {0} was called but is not connected.", slotId);
                return;
            }

            var args = new object[paramCount];

            for (var i = 0; i < paramCount; ++i)
                args[i] = FromNativeObject(paramTypes[i], paramValues[i]);

            connection.Callback.DynamicInvoke(args);
        }

        private static object FromNativeObject(string paramType, IntPtr paramValue)
        {
            Console.WriteLine("Converting from " + paramType);

            switch (paramType)
            {
                case "QString":
                    return QString_Utf16(paramValue);
                default:
                    Trace.TraceError("Cannot convert from native type " + paramType + " to C# object.");
                    return null;
            }
        }

        private static void SlotDisconnected(int slotId)
        {
            Console.WriteLine("Slot disconnected: " + slotId);
            // Make sure we free up the reference to the delegate
            Connections.Remove(slotId);
        }

        public static void Connect(IntPtr sender, string signal, Delegate callback)
        {
            var method = callback.GetType().GetMethod("Invoke");

            var slotId = ConnectionManager_Connect(sender, signal, method.GetParameters().Length);
            Console.WriteLine("Connected as slot id " + slotId);

            Trace.Assert(!Connections.ContainsKey(slotId),
                         "Slot id " + slotId + " was assigned to connection, but is already in use.");

            var connection = new Connection
            {
                Callback = callback,
                Sender = sender,
                Signal = signal
            };
            Connections[slotId] = connection;
        }

        public static void Disconnect(IntPtr sender, string signal, Delegate callback)
        {
            foreach (var entry in Connections)
            {
                var connection = entry.Value;
                if (connection.Sender == sender
                    && connection.Callback == callback
                    && connection.Signal == signal)
                {
                    Connections.Remove(entry.Key);
                    ConnectionManager_Disconnect(entry.Key);
                    return;
                }
            }
        }

        #region PInvoke
        [DllImport("NativeEngine")]
        private static extern int ConnectionManager_Connect(IntPtr sender, string signalName, int paramCount);

        [DllImport("NativeEngine")]
        private static extern bool ConnectionManager_Disconnect(int slotId);

        [DllImport("NativeEngine")]
        private static extern int ConnectionManager_GetConnectionCount();

        [DllImport("NativeEngine")]
        private static extern void ConnectionManager_SetCallSlotCallback(IntPtr callback);

        [DllImport("NativeEngine")]
        private static extern IntPtr ConnectionManager_GetCallSlotCallback();

        [DllImport("NativeEngine")]
        private static extern void ConnectionManager_SetDisconnectCallback(IntPtr callback);

        [DllImport("NativeEngine")]
        private static extern IntPtr ConnectionManager_GetDisconnectCallback();

        [DllImport("NativeEngine", CharSet = CharSet.Unicode)]
        private static extern string QString_Utf16(IntPtr paramValue);

        [DllImport("NativeEngine")]
        private static extern void QString_Free(IntPtr paramValue);
        #endregion
    }

    internal static class EmitExtensions
    {

        public static MethodBuilder Implement(this TypeBuilder typeBuilder, MethodInfo info)
        {
            var paramTypes = new Type[info.GetParameters().Length];
            for (var i = 0; i < paramTypes.Length; ++i)
                paramTypes[i] = info.GetParameters()[i].ParameterType;

            var methodAttributes = info.Attributes & ~MethodAttributes.Abstract;
            return typeBuilder.DefineMethod(info.Name, methodAttributes, info.CallingConvention, info.ReturnType, paramTypes);
        }

    }
}