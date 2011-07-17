/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.4
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace EvilTemple.NativeEngineInterop.Generated {

using System;
using System.Runtime.InteropServices;

class NativeEngineInteropPINVOKE {

  protected class SWIGExceptionHelper {

    public delegate void ExceptionDelegate(string message);
    public delegate void ExceptionArgumentDelegate(string message, string paramName);

    static ExceptionDelegate applicationDelegate = new ExceptionDelegate(SetPendingApplicationException);
    static ExceptionDelegate arithmeticDelegate = new ExceptionDelegate(SetPendingArithmeticException);
    static ExceptionDelegate divideByZeroDelegate = new ExceptionDelegate(SetPendingDivideByZeroException);
    static ExceptionDelegate indexOutOfRangeDelegate = new ExceptionDelegate(SetPendingIndexOutOfRangeException);
    static ExceptionDelegate invalidCastDelegate = new ExceptionDelegate(SetPendingInvalidCastException);
    static ExceptionDelegate invalidOperationDelegate = new ExceptionDelegate(SetPendingInvalidOperationException);
    static ExceptionDelegate ioDelegate = new ExceptionDelegate(SetPendingIOException);
    static ExceptionDelegate nullReferenceDelegate = new ExceptionDelegate(SetPendingNullReferenceException);
    static ExceptionDelegate outOfMemoryDelegate = new ExceptionDelegate(SetPendingOutOfMemoryException);
    static ExceptionDelegate overflowDelegate = new ExceptionDelegate(SetPendingOverflowException);
    static ExceptionDelegate systemDelegate = new ExceptionDelegate(SetPendingSystemException);

    static ExceptionArgumentDelegate argumentDelegate = new ExceptionArgumentDelegate(SetPendingArgumentException);
    static ExceptionArgumentDelegate argumentNullDelegate = new ExceptionArgumentDelegate(SetPendingArgumentNullException);
    static ExceptionArgumentDelegate argumentOutOfRangeDelegate = new ExceptionArgumentDelegate(SetPendingArgumentOutOfRangeException);

    [DllImport("NativeEngine", EntryPoint="SWIGRegisterExceptionCallbacks_NativeEngineInterop")]
    public static extern void SWIGRegisterExceptionCallbacks_NativeEngineInterop(
                                ExceptionDelegate applicationDelegate,
                                ExceptionDelegate arithmeticDelegate,
                                ExceptionDelegate divideByZeroDelegate, 
                                ExceptionDelegate indexOutOfRangeDelegate, 
                                ExceptionDelegate invalidCastDelegate,
                                ExceptionDelegate invalidOperationDelegate,
                                ExceptionDelegate ioDelegate,
                                ExceptionDelegate nullReferenceDelegate,
                                ExceptionDelegate outOfMemoryDelegate, 
                                ExceptionDelegate overflowDelegate, 
                                ExceptionDelegate systemExceptionDelegate);

    [DllImport("NativeEngine", EntryPoint="SWIGRegisterExceptionArgumentCallbacks_NativeEngineInterop")]
    public static extern void SWIGRegisterExceptionCallbacksArgument_NativeEngineInterop(
                                ExceptionArgumentDelegate argumentDelegate,
                                ExceptionArgumentDelegate argumentNullDelegate,
                                ExceptionArgumentDelegate argumentOutOfRangeDelegate);

    static void SetPendingApplicationException(string message) {
      SWIGPendingException.Set(new System.ApplicationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArithmeticException(string message) {
      SWIGPendingException.Set(new System.ArithmeticException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingDivideByZeroException(string message) {
      SWIGPendingException.Set(new System.DivideByZeroException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIndexOutOfRangeException(string message) {
      SWIGPendingException.Set(new System.IndexOutOfRangeException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidCastException(string message) {
      SWIGPendingException.Set(new System.InvalidCastException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingInvalidOperationException(string message) {
      SWIGPendingException.Set(new System.InvalidOperationException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingIOException(string message) {
      SWIGPendingException.Set(new System.IO.IOException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingNullReferenceException(string message) {
      SWIGPendingException.Set(new System.NullReferenceException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOutOfMemoryException(string message) {
      SWIGPendingException.Set(new System.OutOfMemoryException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingOverflowException(string message) {
      SWIGPendingException.Set(new System.OverflowException(message, SWIGPendingException.Retrieve()));
    }
    static void SetPendingSystemException(string message) {
      SWIGPendingException.Set(new System.SystemException(message, SWIGPendingException.Retrieve()));
    }

    static void SetPendingArgumentException(string message, string paramName) {
      SWIGPendingException.Set(new System.ArgumentException(message, paramName, SWIGPendingException.Retrieve()));
    }
    static void SetPendingArgumentNullException(string message, string paramName) {
      Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new System.ArgumentNullException(paramName, message));
    }
    static void SetPendingArgumentOutOfRangeException(string message, string paramName) {
      Exception e = SWIGPendingException.Retrieve();
      if (e != null) message = message + " Inner Exception: " + e.Message;
      SWIGPendingException.Set(new System.ArgumentOutOfRangeException(paramName, message));
    }

    static SWIGExceptionHelper() {
      SWIGRegisterExceptionCallbacks_NativeEngineInterop(
                                applicationDelegate,
                                arithmeticDelegate,
                                divideByZeroDelegate,
                                indexOutOfRangeDelegate,
                                invalidCastDelegate,
                                invalidOperationDelegate,
                                ioDelegate,
                                nullReferenceDelegate,
                                outOfMemoryDelegate,
                                overflowDelegate,
                                systemDelegate);

      SWIGRegisterExceptionCallbacksArgument_NativeEngineInterop(
                                argumentDelegate,
                                argumentNullDelegate,
                                argumentOutOfRangeDelegate);
    }
  }

  protected static SWIGExceptionHelper swigExceptionHelper = new SWIGExceptionHelper();

  public class SWIGPendingException {
    [ThreadStatic]
    private static Exception pendingException = null;
    private static int numExceptionsPending = 0;

    public static bool Pending {
      get {
        bool pending = false;
        if (numExceptionsPending > 0)
          if (pendingException != null)
            pending = true;
        return pending;
      } 
    }

    public static void Set(Exception e) {
      if (pendingException != null)
        throw new ApplicationException("FATAL: An earlier pending exception from unmanaged code was missed and thus not thrown (" + pendingException.ToString() + ")", e);
      pendingException = e;
      lock(typeof(NativeEngineInteropPINVOKE)) {
        numExceptionsPending++;
      }
    }

    public static Exception Retrieve() {
      Exception e = null;
      if (numExceptionsPending > 0) {
        if (pendingException != null) {
          e = pendingException;
          pendingException = null;
          lock(typeof(NativeEngineInteropPINVOKE)) {
            numExceptionsPending--;
          }
        }
      }
      return e;
    }
  }


  protected class SWIGStringHelper {

    public delegate string SWIGStringDelegate(string message);
    static SWIGStringDelegate stringDelegate = new SWIGStringDelegate(CreateString);

    [DllImport("NativeEngine", EntryPoint="SWIGRegisterStringCallback_NativeEngineInterop")]
    public static extern void SWIGRegisterStringCallback_NativeEngineInterop(SWIGStringDelegate stringDelegate);

    static string CreateString(string cString) {
      return cString;
    }

    static SWIGStringHelper() {
      SWIGRegisterStringCallback_NativeEngineInterop(stringDelegate);
    }
  }

  static protected SWIGStringHelper swigStringHelper = new SWIGStringHelper();


  static NativeEngineInteropPINVOKE() {
  }


  [DllImport("NativeEngine", EntryPoint="CSharp_delete_QByteArray")]
  public static extern void delete_QByteArray(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_QByteArray_size")]
  public static extern int QByteArray_size(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_QByteArray_constData")]
  public static extern IntPtr QByteArray_constData(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_setSelectionData")]
  public static extern void MovableObject_setSelectionData(HandleRef jarg1, long jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_hasSelectionData")]
  public static extern bool MovableObject_hasSelectionData(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_clearSelectionData")]
  public static extern void MovableObject_clearSelectionData(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_getSelectionId")]
  public static extern long MovableObject_getSelectionId(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_getSelectionRadius")]
  public static extern float MovableObject_getSelectionRadius(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_MovableObject_getSelectionHeight")]
  public static extern float MovableObject_getSelectionHeight(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Node_setInitialState")]
  public static extern void Node_setInitialState(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_X_set")]
  public static extern void Vector3_X_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_X_get")]
  public static extern float Vector3_X_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_Y_set")]
  public static extern void Vector3_Y_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_Y_get")]
  public static extern float Vector3_Y_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_Z_set")]
  public static extern void Vector3_Z_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Vector3_Z_get")]
  public static extern float Vector3_Z_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_Vector3")]
  public static extern IntPtr new_Vector3(float jarg1, float jarg2, float jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_Vector3")]
  public static extern void delete_Vector3(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_X_set")]
  public static extern void Quaternion_X_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_X_get")]
  public static extern float Quaternion_X_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_Y_set")]
  public static extern void Quaternion_Y_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_Y_get")]
  public static extern float Quaternion_Y_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_Z_set")]
  public static extern void Quaternion_Z_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_Z_get")]
  public static extern float Quaternion_Z_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_W_set")]
  public static extern void Quaternion_W_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_W_get")]
  public static extern float Quaternion_W_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_Quaternion__SWIG_0")]
  public static extern IntPtr new_Quaternion__SWIG_0();

  [DllImport("NativeEngine", EntryPoint="CSharp_new_Quaternion__SWIG_1")]
  public static extern IntPtr new_Quaternion__SWIG_1(float jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Quaternion_FromAngleAxis")]
  public static extern void Quaternion_FromAngleAxis(HandleRef jarg1, float jarg2, HandleRef jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_Quaternion")]
  public static extern void delete_Quaternion(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_attachObject")]
  public static extern void SceneNode_attachObject(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_numAttachedObjects")]
  public static extern ushort SceneNode_numAttachedObjects(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getAttachedObject")]
  public static extern IntPtr SceneNode_getAttachedObject(HandleRef jarg1, ushort jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_createChildSceneNode__SWIG_0")]
  public static extern IntPtr SceneNode_createChildSceneNode__SWIG_0(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_createChildSceneNode__SWIG_1")]
  public static extern IntPtr SceneNode_createChildSceneNode__SWIG_1(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getParentSceneNode")]
  public static extern IntPtr SceneNode_getParentSceneNode(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_removeChild")]
  public static extern void SceneNode_removeChild(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_addChild")]
  public static extern void SceneNode_addChild(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_setVisible__SWIG_0")]
  public static extern void SceneNode_setVisible__SWIG_0(HandleRef jarg1, bool jarg2, bool jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_setVisible__SWIG_1")]
  public static extern void SceneNode_setVisible__SWIG_1(HandleRef jarg1, bool jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getName")]
  public static extern string SceneNode_getName(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_setPosition")]
  public static extern void SceneNode_setPosition(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getPosition")]
  public static extern IntPtr SceneNode_getPosition(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_setOrientation")]
  public static extern void SceneNode_setOrientation(HandleRef jarg1, float jarg2, float jarg3, float jarg4, float jarg5);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getOrientation")]
  public static extern IntPtr SceneNode_getOrientation(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_setScale")]
  public static extern void SceneNode_setScale(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getScale")]
  public static extern IntPtr SceneNode_getScale(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_numChildren")]
  public static extern ushort SceneNode_numChildren(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_getChild")]
  public static extern IntPtr SceneNode_getChild(HandleRef jarg1, ushort jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_r_set")]
  public static extern void ColourValue_r_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_r_get")]
  public static extern float ColourValue_r_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_g_set")]
  public static extern void ColourValue_g_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_g_get")]
  public static extern float ColourValue_g_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_b_set")]
  public static extern void ColourValue_b_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_b_get")]
  public static extern float ColourValue_b_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_a_set")]
  public static extern void ColourValue_a_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ColourValue_a_get")]
  public static extern float ColourValue_a_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setType")]
  public static extern void Light_setType(HandleRef jarg1, int jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getType")]
  public static extern int Light_getType(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setDiffuseColour")]
  public static extern void Light_setDiffuseColour(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getDiffuseColour")]
  public static extern IntPtr Light_getDiffuseColour(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setSpecularColour")]
  public static extern void Light_setSpecularColour(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getSpecularColour")]
  public static extern IntPtr Light_getSpecularColour(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setAttenuation")]
  public static extern void Light_setAttenuation(HandleRef jarg1, float jarg2, float jarg3, float jarg4, float jarg5);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getAttenuationRange")]
  public static extern float Light_getAttenuationRange(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getAttenuationConstant")]
  public static extern float Light_getAttenuationConstant(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getAttenuationLinear")]
  public static extern float Light_getAttenuationLinear(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getAttenuationQuadric")]
  public static extern float Light_getAttenuationQuadric(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setPosition")]
  public static extern void Light_setPosition(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getPosition")]
  public static extern IntPtr Light_getPosition(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setDirection")]
  public static extern void Light_setDirection(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getDirection")]
  public static extern IntPtr Light_getDirection(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setSpotlightRange")]
  public static extern void Light_setSpotlightRange(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getSpotlightInnerAngle")]
  public static extern float Light_getSpotlightInnerAngle(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getSpotlightOuterAngle")]
  public static extern float Light_getSpotlightOuterAngle(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getSpotlightFalloff")]
  public static extern float Light_getSpotlightFalloff(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_setPowerScale")]
  public static extern void Light_setPowerScale(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_getPowerScale")]
  public static extern float Light_getPowerScale(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_setTranslate")]
  public static extern void TransformKeyFrame_setTranslate(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_getTranslate")]
  public static extern IntPtr TransformKeyFrame_getTranslate(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_setScale")]
  public static extern void TransformKeyFrame_setScale(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_getScale")]
  public static extern IntPtr TransformKeyFrame_getScale(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_setRotation")]
  public static extern void TransformKeyFrame_setRotation(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_TransformKeyFrame_getRotation")]
  public static extern IntPtr TransformKeyFrame_getRotation(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NodeAnimationTrack_createNodeKeyFrame")]
  public static extern IntPtr NodeAnimationTrack_createNodeKeyFrame(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Animation_createNodeTrack")]
  public static extern IntPtr Animation_createNodeTrack(HandleRef jarg1, ushort jarg2, HandleRef jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_AnimationState_setEnabled")]
  public static extern void AnimationState_setEnabled(HandleRef jarg1, bool jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_AnimationState_getEnabled")]
  public static extern bool AnimationState_getEnabled(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_AnimationState_setLoop")]
  public static extern void AnimationState_setLoop(HandleRef jarg1, bool jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_AnimationState_getLoop")]
  public static extern bool AnimationState_getLoop(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_AnimationState_addTime")]
  public static extern void AnimationState_addTime(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateEntity__SWIG_0")]
  public static extern IntPtr SceneManager_CreateEntity__SWIG_0(HandleRef jarg1, string jarg2, string jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateEntity__SWIG_1")]
  public static extern IntPtr SceneManager_CreateEntity__SWIG_1(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateSceneNode__SWIG_0")]
  public static extern IntPtr SceneManager_CreateSceneNode__SWIG_0(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateSceneNode__SWIG_1")]
  public static extern IntPtr SceneManager_CreateSceneNode__SWIG_1(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_GetRootSceneNode")]
  public static extern IntPtr SceneManager_GetRootSceneNode(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateLight__SWIG_0")]
  public static extern IntPtr SceneManager_CreateLight__SWIG_0(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateLight__SWIG_1")]
  public static extern IntPtr SceneManager_CreateLight__SWIG_1(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateAnimation")]
  public static extern IntPtr SceneManager_CreateAnimation(HandleRef jarg1, string jarg2, float jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_GetAnimation")]
  public static extern IntPtr SceneManager_GetAnimation(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_HasAnimation")]
  public static extern bool SceneManager_HasAnimation(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_DestroyAnimation")]
  public static extern void SceneManager_DestroyAnimation(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_CreateAnimationState")]
  public static extern IntPtr SceneManager_CreateAnimationState(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_destroyAnimationState")]
  public static extern void SceneManager_destroyAnimationState(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneManager_getAnimationState")]
  public static extern IntPtr SceneManager_getAnimationState(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Camera_SetPosition")]
  public static extern void Camera_SetPosition(HandleRef jarg1, float jarg2, float jarg3, float jarg4);

  [DllImport("NativeEngine", EntryPoint="CSharp_Camera_GetPosition")]
  public static extern IntPtr Camera_GetPosition(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Camera_Move")]
  public static extern void Camera_Move(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_GroundDisc_setMaterial")]
  public static extern void GroundDisc_setMaterial(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResult_id_set")]
  public static extern void PickResult_id_set(HandleRef jarg1, long jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResult_id_get")]
  public static extern long PickResult_id_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResult_distance_set")]
  public static extern void PickResult_distance_set(HandleRef jarg1, float jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResult_distance_get")]
  public static extern float PickResult_distance_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_PickResult")]
  public static extern IntPtr new_PickResult();

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_PickResult")]
  public static extern void delete_PickResult(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResultList_at")]
  public static extern IntPtr PickResultList_at(HandleRef jarg1, int jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_PickResultList_size")]
  public static extern int PickResultList_size(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_PickResultList")]
  public static extern IntPtr new_PickResultList();

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_PickResultList")]
  public static extern void delete_PickResultList(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_CreateBackgroundMap")]
  public static extern IntPtr Scene_CreateBackgroundMap(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_CreateGroundDisc")]
  public static extern IntPtr Scene_CreateGroundDisc(HandleRef jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_GetMainCamera")]
  public static extern IntPtr Scene_GetMainCamera(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_GetCameraOrigin")]
  public static extern IntPtr Scene_GetCameraOrigin(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_pick")]
  public static extern IntPtr Scene_pick(HandleRef jarg1, float jarg2, float jarg3);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_logCallback_set")]
  public static extern void NativeEngineSettings_logCallback_set(HandleRef jarg1, /* imtype* */ IntPtr jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_logCallback_get")]
  public static extern /* imtype* out */ IntPtr NativeEngineSettings_logCallback_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_argc_set")]
  public static extern void NativeEngineSettings_argc_set(HandleRef jarg1, int jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_argc_get")]
  public static extern int NativeEngineSettings_argc_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_argv_set")]
  public static extern void NativeEngineSettings_argv_set(HandleRef jarg1, HandleRef jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngineSettings_argv_get")]
  public static extern IntPtr NativeEngineSettings_argv_get(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_NativeEngineSettings")]
  public static extern IntPtr new_NativeEngineSettings();

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_NativeEngineSettings")]
  public static extern void delete_NativeEngineSettings(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_new_NativeEngine")]
  public static extern IntPtr new_NativeEngine(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_delete_NativeEngine")]
  public static extern void delete_NativeEngine(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_processEvents")]
  public static extern void NativeEngine_processEvents(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_renderFrame")]
  public static extern void NativeEngine_renderFrame(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_mainScene")]
  public static extern IntPtr NativeEngine_mainScene(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_interfaceRoot")]
  public static extern /* imtype* out */ IntPtr NativeEngine_interfaceRoot(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_windowWidth")]
  public static extern int NativeEngine_windowWidth(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_NativeEngine_windowHeight")]
  public static extern int NativeEngine_windowHeight(HandleRef jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_addZipArchive")]
  public static extern void ResourceManager_addZipArchive(string jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_addDirectory")]
  public static extern void ResourceManager_addDirectory(string jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_remove")]
  public static extern void ResourceManager_remove(string jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_initializeGroup")]
  public static extern void ResourceManager_initializeGroup(string jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_read__SWIG_0")]
  public static extern IntPtr ResourceManager_read__SWIG_0(string jarg1, string jarg2);

  [DllImport("NativeEngine", EntryPoint="CSharp_ResourceManager_read__SWIG_1")]
  public static extern IntPtr ResourceManager_read__SWIG_1(string jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Entity_SWIGUpcast")]
  public static extern IntPtr Entity_SWIGUpcast(IntPtr jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_SceneNode_SWIGUpcast")]
  public static extern IntPtr SceneNode_SWIGUpcast(IntPtr jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Light_SWIGUpcast")]
  public static extern IntPtr Light_SWIGUpcast(IntPtr jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_GroundDisc_SWIGUpcast")]
  public static extern IntPtr GroundDisc_SWIGUpcast(IntPtr jarg1);

  [DllImport("NativeEngine", EntryPoint="CSharp_Scene_SWIGUpcast")]
  public static extern IntPtr Scene_SWIGUpcast(IntPtr jarg1);
}

}
