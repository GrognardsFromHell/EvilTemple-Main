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

public class TransformKeyFrame : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal TransformKeyFrame(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(TransformKeyFrame obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          throw new MethodAccessException("C++ destructor does not have public access");
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public void setTranslate(Vector3 trans) {
    NativeEngineInteropPINVOKE.TransformKeyFrame_setTranslate(swigCPtr, Vector3.getCPtr(trans));
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Vector3 getTranslate() {
    Vector3 ret = new Vector3(NativeEngineInteropPINVOKE.TransformKeyFrame_getTranslate(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setScale(Vector3 scale) {
    NativeEngineInteropPINVOKE.TransformKeyFrame_setScale(swigCPtr, Vector3.getCPtr(scale));
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Vector3 getScale() {
    Vector3 ret = new Vector3(NativeEngineInteropPINVOKE.TransformKeyFrame_getScale(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setRotation(Quaternion quaternion) {
    NativeEngineInteropPINVOKE.TransformKeyFrame_setRotation(swigCPtr, Quaternion.getCPtr(quaternion));
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Quaternion getRotation() {
    Quaternion ret = new Quaternion(NativeEngineInteropPINVOKE.TransformKeyFrame_getRotation(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}