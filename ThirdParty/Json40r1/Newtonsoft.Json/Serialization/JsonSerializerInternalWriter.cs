#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
#if !(NET35 || NET20 || WINDOWS_PHONE)
#endif

namespace Newtonsoft.Json.Serialization
{
    internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
    {
        private JsonSerializerProxy _internalSerializer;
        private List<object> _serializeStack;

        private List<object> SerializeStack
        {
            get
            {
                if (_serializeStack == null)
                    _serializeStack = new List<object>();

                return _serializeStack;
            }
        }

        public JsonSerializerInternalWriter(JsonSerializer serializer)
            : base(serializer)
        {
        }

        public void Serialize(JsonWriter jsonWriter, object value, bool forceType)
        {
            if (jsonWriter == null)
                throw new ArgumentNullException("jsonWriter");

            SerializeValue(jsonWriter, value, GetContractSafe(value), null, null, forceType);
        }

        private JsonSerializerProxy GetInternalSerializer()
        {
            if (_internalSerializer == null)
                _internalSerializer = new JsonSerializerProxy(this);

            return _internalSerializer;
        }

        private JsonContract GetContractSafe(object value)
        {
            if (value == null)
                return null;

            return Serializer.ContractResolver.ResolveContract(value.GetType());
        }

        private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member,
                                    JsonContract collectionValueContract, bool forceType = false)
        {
            var converter = (member != null) ? member.Converter : null;

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if ((converter != null
                 || ((converter = valueContract.Converter) != null)
                 || ((converter = Serializer.GetMatchingConverter(valueContract.UnderlyingType)) != null)
                 || ((converter = valueContract.InternalConverter) != null))
                && converter.CanWrite)
            {
                SerializeConvertable(writer, converter, value, valueContract);
            }
            else if (valueContract is JsonPrimitiveContract)
            {
                writer.WriteValue(value);
            }
            else if (valueContract is JsonStringContract)
            {
                SerializeString(writer, value, (JsonStringContract) valueContract);
            }
            else if (valueContract is JsonObjectContract)
            {
                SerializeObject(writer, value, (JsonObjectContract) valueContract, member, collectionValueContract, forceType);
            }
            else if (valueContract is JsonDictionaryContract)
            {
                var dictionaryContract = (JsonDictionaryContract) valueContract;
                SerializeDictionary(writer, dictionaryContract.CreateWrapper(value), dictionaryContract, member,
                                    collectionValueContract);
            }
            else if (valueContract is JsonArrayContract)
            {
                var arrayContract = (JsonArrayContract) valueContract;
                SerializeList(writer, arrayContract.CreateWrapper(value), arrayContract, member, collectionValueContract);
            }
            else if (valueContract is JsonLinqContract)
            {
                ((JToken) value).WriteTo(writer,
                                         (Serializer.Converters != null) ? Serializer.Converters.ToArray() : null);
            }
#if !SILVERLIGHT && !PocketPC
            else if (valueContract is JsonISerializableContract)
            {
                SerializeISerializable(writer, (ISerializable) value, (JsonISerializableContract) valueContract);
            }
#endif
#if !(NET35 || NET20 || WINDOWS_PHONE)
            else if (valueContract is JsonDynamicContract)
            {
                SerializeDynamic(writer, (IDynamicMetaObjectProvider) value, (JsonDynamicContract) valueContract);
            }
#endif
        }

        private bool ShouldWriteReference(object value, JsonProperty property, JsonContract contract)
        {
            if (value == null)
                return false;
            if (contract is JsonPrimitiveContract)
                return false;

            bool? isReference = null;

            // value could be coming from a dictionary or array and not have a property
            if (property != null)
                isReference = property.IsReference;

            if (isReference == null)
                isReference = contract.IsReference;

            if (isReference == null)
            {
                if (contract is JsonArrayContract)
                    isReference = HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays);
                else
                    isReference = HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects);
            }

            if (!isReference.Value)
                return false;

            return Serializer.ReferenceResolver.IsReferenced(value);
        }

        private void WriteMemberInfoProperty(JsonWriter writer, object memberValue, JsonProperty property,
                                             JsonContract contract)
        {
            var propertyName = property.PropertyName;
            var defaultValue = property.DefaultValue;

            if (property.NullValueHandling.GetValueOrDefault(Serializer.NullValueHandling) == NullValueHandling.Ignore &&
                memberValue == null)
                return;

            if (property.DefaultValueHandling.GetValueOrDefault(Serializer.DefaultValueHandling) ==
                DefaultValueHandling.Ignore && AreEqual(memberValue, defaultValue))
                return;

            if (ShouldWriteReference(memberValue, property, contract))
            {
                writer.WritePropertyName(propertyName);
                WriteReference(writer, memberValue);
                return;
            }

            if (!CheckForCircularReference(memberValue, property.ReferenceLoopHandling, contract))
                return;

            if (memberValue == null && property.Required == Required.Always)
                throw new JsonSerializationException(
                    "Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(
                        CultureInfo.InvariantCulture, property.PropertyName));

            writer.WritePropertyName(propertyName);
            SerializeValue(writer, memberValue, contract, property, null);
        }

        private static bool AreSetEqual(object a, object b)
        {
            var equalsMethod = a.GetType().GetMethod("SetEquals");
            if (equalsMethod == null)
                return false;
            return (bool) equalsMethod.Invoke(a, new[] {b});
        }

        private bool AreEqual(object memberValue, object defaultValue)
        {
            if (Equals(memberValue, defaultValue))
                return true;

            // arrays need special handling
            if (memberValue == null ||
                defaultValue == null ||
                memberValue.GetType() != defaultValue.GetType())
                return false;

            if (AreSetEqual(memberValue, defaultValue))
                return true;

            // Check if two IEnumerable's are equal
            if (!(memberValue is IEnumerable))
                return false;

            var enumerableA = ((IEnumerable) memberValue).GetEnumerator();
            var enumerableB = ((IEnumerable) defaultValue).GetEnumerator();
            
            var aHasValue = enumerableA.MoveNext();
            var bHasValue = enumerableB.MoveNext();

            while (aHasValue && bHasValue)
            {
                if (!Equals(enumerableA.Current, enumerableB.Current))
                    return false;
                aHasValue = enumerableA.MoveNext();
                bHasValue = enumerableB.MoveNext();
            }

            return aHasValue == bHasValue;
        }

        private bool CheckForCircularReference(object value, ReferenceLoopHandling? referenceLoopHandling,
                                               JsonContract contract)
        {
            if (value == null || contract is JsonPrimitiveContract)
                return true;

            if (SerializeStack.IndexOf(value) != -1)
            {
                switch (referenceLoopHandling.GetValueOrDefault(Serializer.ReferenceLoopHandling))
                {
                    case ReferenceLoopHandling.Error:
                        throw new JsonSerializationException(
                            "Self referencing loop detected for type '{0}'.".FormatWith(CultureInfo.InvariantCulture,
                                                                                        value.GetType()));
                    case ReferenceLoopHandling.Ignore:
                        return false;
                    case ReferenceLoopHandling.Serialize:
                        return true;
                    default:
                        throw new InvalidOperationException(
                            "Unexpected ReferenceLoopHandling value: '{0}'".FormatWith(CultureInfo.InvariantCulture,
                                                                                       Serializer.ReferenceLoopHandling));
                }
            }

            return true;
        }

        private void WriteReference(JsonWriter writer, object value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(JsonTypeReflector.RefPropertyName);
            writer.WriteValue(Serializer.ReferenceResolver.GetReference(value));
            writer.WriteEndObject();
        }

        internal static bool TryConvertToString(object value, Type type, out string s)
        {
#if !PocketPC
            var converter = ConvertUtils.GetConverter(type);

            // use the objectType's TypeConverter if it has one and can convert to a string
            if (converter != null
#if !SILVERLIGHT
                && !(converter is ComponentConverter)
#endif
                && converter.GetType() != typeof (TypeConverter))
            {
                if (converter.CanConvertTo(typeof (string)))
                {
#if !SILVERLIGHT
                    s = converter.ConvertToInvariantString(value);
#else
          s = converter.ConvertToString(value);
#endif
                    return true;
                }
            }
#endif

#if SILVERLIGHT || PocketPC
      if (value is Guid || value is Uri || value is TimeSpan)
      {
        s = value.ToString();
        return true;
      }
#endif

            if (value is Type)
            {
                s = ((Type) value).AssemblyQualifiedName;
                return true;
            }

            s = null;
            return false;
        }

        private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
        {
            contract.InvokeOnSerializing(value, Serializer.Context);

            string s;
            TryConvertToString(value, contract.UnderlyingType, out s);
            writer.WriteValue(s);

            contract.InvokeOnSerialized(value, Serializer.Context);
        }

        private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContract collectionValueContract, bool forceType)
        {
            contract.InvokeOnSerializing(value, Serializer.Context);

            SerializeStack.Add(value);
            writer.WriteStartObject();

            var isReference = contract.IsReference ??
                              HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects);
            if (isReference)
            {
                writer.WritePropertyName(JsonTypeReflector.IdPropertyName);
                writer.WriteValue(Serializer.ReferenceResolver.GetReference(value));
            }
            if (forceType || ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
            {
                WriteTypeProperty(writer, contract.UnderlyingType);
            }

            var initialDepth = writer.Top;

            foreach (var property in contract.Properties)
            {
                try
                {
                    if (!property.Ignored && property.Readable && ShouldSerialize(property, value) &&
                        IsSpecified(property, value))
                    {
                        var memberValue = property.ValueProvider.GetValue(value);
                        var memberContract = GetContractSafe(memberValue);

                        WriteMemberInfoProperty(writer, memberValue, property, memberContract);
                    }
                }
                catch (Exception ex)
                {
                    if (IsErrorHandled(value, contract, property.PropertyName, ex))
                        HandleError(writer, initialDepth);
                    else
                        throw;
                }
            }

            writer.WriteEndObject();
            SerializeStack.RemoveAt(SerializeStack.Count - 1);

            contract.InvokeOnSerialized(value, Serializer.Context);
        }

        private void WriteTypeProperty(JsonWriter writer, Type type)
        {
            writer.WritePropertyName(JsonTypeReflector.TypePropertyName);

            // See if for the given type, a predefined equivalence mapping exists.
            string typeName;
            if (!Serializer.TypeMapping.TryGetValue(type, out typeName))
                typeName = ReflectionUtils.GetTypeName(type, Serializer.TypeNameAssemblyFormat);

            writer.WriteValue(typeName);
        }

        private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
        {
            return ((value & flag) == flag);
        }

        private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
        {
            return ((value & flag) == flag);
        }

        private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value,
                                          JsonContract contract)
        {
            if (ShouldWriteReference(value, null, contract))
            {
                WriteReference(writer, value);
            }
            else
            {
                if (!CheckForCircularReference(value, null, contract))
                    return;

                SerializeStack.Add(value);

                converter.WriteJson(writer, value, GetInternalSerializer());

                SerializeStack.RemoveAt(SerializeStack.Count - 1);
            }
        }

        private void SerializeList(JsonWriter writer, IWrappedCollection values, JsonArrayContract contract,
                                   JsonProperty member, JsonContract collectionValueContract)
        {
            contract.InvokeOnSerializing(values.UnderlyingCollection, Serializer.Context);

            SerializeStack.Add(values.UnderlyingCollection);

            var isReference = contract.IsReference ??
                              HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Arrays);
            var includeTypeDetails = ShouldWriteType(TypeNameHandling.Arrays, contract, member, collectionValueContract);

            if (isReference || includeTypeDetails)
            {
                writer.WriteStartObject();

                if (isReference)
                {
                    writer.WritePropertyName(JsonTypeReflector.IdPropertyName);
                    writer.WriteValue(Serializer.ReferenceResolver.GetReference(values.UnderlyingCollection));
                }
                if (includeTypeDetails)
                {
                    WriteTypeProperty(writer, values.UnderlyingCollection.GetType());
                }
                writer.WritePropertyName(JsonTypeReflector.ArrayValuesPropertyName);
            }

            var childValuesContract =
                Serializer.ContractResolver.ResolveContract(contract.CollectionItemType ?? typeof (object));

            writer.WriteStartArray();

            var initialDepth = writer.Top;

            var index = 0;
            // note that an error in the IEnumerable won't be caught
            foreach (var value in values)
            {
                try
                {
                    var valueContract = GetContractSafe(value);

                    if (ShouldWriteReference(value, null, valueContract))
                    {
                        WriteReference(writer, value);
                    }
                    else
                    {
                        if (CheckForCircularReference(value, null, contract))
                        {
                            SerializeValue(writer, value, valueContract, null, childValuesContract);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (IsErrorHandled(values.UnderlyingCollection, contract, index, ex))
                        HandleError(writer, initialDepth);
                    else
                        throw;
                }
                finally
                {
                    index++;
                }
            }

            writer.WriteEndArray();

            if (isReference || includeTypeDetails)
            {
                writer.WriteEndObject();
            }

            SerializeStack.RemoveAt(SerializeStack.Count - 1);

            contract.InvokeOnSerialized(values.UnderlyingCollection, Serializer.Context);
        }

#if !SILVERLIGHT && !PocketPC
#if !NET20
        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute")]
        [SecuritySafeCritical]
#endif
        private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract)
        {
            contract.InvokeOnSerializing(value, Serializer.Context);
            SerializeStack.Add(value);

            writer.WriteStartObject();

            var serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
            value.GetObjectData(serializationInfo, Serializer.Context);

            foreach (var serializationEntry in serializationInfo)
            {
                writer.WritePropertyName(serializationEntry.Name);
                SerializeValue(writer, serializationEntry.Value, GetContractSafe(serializationEntry.Value), null, null);
            }

            writer.WriteEndObject();

            SerializeStack.RemoveAt(SerializeStack.Count - 1);
            contract.InvokeOnSerialized(value, Serializer.Context);
        }
#endif

#if !(NET35 || NET20 || WINDOWS_PHONE)
        private void SerializeDynamic(JsonWriter writer, IDynamicMetaObjectProvider value, JsonDynamicContract contract)
        {
            contract.InvokeOnSerializing(value, Serializer.Context);
            SerializeStack.Add(value);

            writer.WriteStartObject();

            foreach (var memberName in value.GetDynamicMemberNames())
            {
                object memberValue;
                if (value.TryGetMember(memberName, out memberValue))
                {
                    writer.WritePropertyName(memberName);
                    SerializeValue(writer, memberValue, GetContractSafe(memberValue), null, null);
                }
            }

            writer.WriteEndObject();

            SerializeStack.RemoveAt(SerializeStack.Count - 1);
            contract.InvokeOnSerialized(value, Serializer.Context);
        }
#endif

        private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member,
                                     JsonContract collectionValueContract)
        {
            if (HasFlag(((member != null) ? member.TypeNameHandling : null) ?? Serializer.TypeNameHandling,
                        typeNameHandlingFlag))
                return true;

            if (member != null)
            {
                if ((member.TypeNameHandling ?? Serializer.TypeNameHandling) == TypeNameHandling.Auto &&
                    contract.UnderlyingType != member.PropertyType)
                    return true;
            }
            else if (collectionValueContract != null)
            {
                if (Serializer.TypeNameHandling == TypeNameHandling.Auto &&
                    contract.UnderlyingType != collectionValueContract.UnderlyingType)
                    return true;
            }

            return false;
        }

        private void SerializeDictionary(JsonWriter writer, IWrappedDictionary values, JsonDictionaryContract contract,
                                         JsonProperty member, JsonContract collectionValueContract)
        {
            contract.InvokeOnSerializing(values.UnderlyingDictionary, Serializer.Context);

            SerializeStack.Add(values.UnderlyingDictionary);
            writer.WriteStartObject();

            var isReference = contract.IsReference ??
                              HasFlag(Serializer.PreserveReferencesHandling, PreserveReferencesHandling.Objects);
            if (isReference)
            {
                writer.WritePropertyName(JsonTypeReflector.IdPropertyName);
                writer.WriteValue(Serializer.ReferenceResolver.GetReference(values.UnderlyingDictionary));
            }
            if (ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionValueContract))
            {
                WriteTypeProperty(writer, values.UnderlyingDictionary.GetType());
            }

            var childValuesContract =
                Serializer.ContractResolver.ResolveContract(contract.DictionaryValueType ?? typeof (object));

            var initialDepth = writer.Top;

            // Mono Unity 3.0 fix
            IDictionary d = values;

            foreach (DictionaryEntry entry in d)
            {
                var propertyName = GetPropertyName(entry);

                try
                {
                    var value = entry.Value;
                    var valueContract = GetContractSafe(value);

                    if (ShouldWriteReference(value, null, valueContract))
                    {
                        writer.WritePropertyName(propertyName);
                        WriteReference(writer, value);
                    }
                    else
                    {
                        if (!CheckForCircularReference(value, null, contract))
                            continue;

                        writer.WritePropertyName(propertyName);

                        SerializeValue(writer, value, valueContract, null, childValuesContract);
                    }
                }
                catch (Exception ex)
                {
                    if (IsErrorHandled(values.UnderlyingDictionary, contract, propertyName, ex))
                        HandleError(writer, initialDepth);
                    else
                        throw;
                }
            }

            writer.WriteEndObject();
            SerializeStack.RemoveAt(SerializeStack.Count - 1);

            contract.InvokeOnSerialized(values.UnderlyingDictionary, Serializer.Context);
        }

        private string GetPropertyName(DictionaryEntry entry)
        {
            string propertyName;

            if (entry.Key is IConvertible)
                return Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
            else if (TryConvertToString(entry.Key, entry.Key.GetType(), out propertyName))
                return propertyName;
            else
                return entry.Key.ToString();
        }

        private void HandleError(JsonWriter writer, int initialDepth)
        {
            ClearErrorContext();

            while (writer.Top > initialDepth)
            {
                writer.WriteEnd();
            }
        }

        private bool ShouldSerialize(JsonProperty property, object target)
        {
            if (property.ShouldSerialize == null)
                return true;

            return property.ShouldSerialize(target);
        }

        private bool IsSpecified(JsonProperty property, object target)
        {
            if (property.GetIsSpecified == null)
                return true;

            return property.GetIsSpecified(target);
        }
    }
}