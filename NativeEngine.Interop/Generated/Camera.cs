/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.4
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace EvilTemple.NativeEngineInterop {

using System;
using System.Runtime.InteropServices;

public class Camera : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal Camera(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(Camera obj) {
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

  public void SetPosition(float x, float y, float z) {
    NativeEngineInteropPINVOKE.Camera_SetPosition(swigCPtr, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Vector3 GetPosition() {
    Vector3 ret = new Vector3(NativeEngineInteropPINVOKE.Camera_GetPosition(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Move(Vector3 vec) {
    NativeEngineInteropPINVOKE.Camera_Move(swigCPtr, Vector3.getCPtr(vec));
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
