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

public class SceneNode : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal SceneNode(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(SceneNode obj) {
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

  public void AttachObject(MovableObject obj) {
    NativeEngineInteropPINVOKE.SceneNode_AttachObject(swigCPtr, MovableObject.getCPtr(obj));
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public SceneNode CreateChildSceneNode() {
    IntPtr cPtr = NativeEngineInteropPINVOKE.SceneNode_CreateChildSceneNode__SWIG_0(swigCPtr);
    SceneNode ret = (cPtr == IntPtr.Zero) ? null : new SceneNode(cPtr, false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public SceneNode CreateChildSceneNode(string name) {
    IntPtr cPtr = NativeEngineInteropPINVOKE.SceneNode_CreateChildSceneNode__SWIG_1(swigCPtr, name);
    SceneNode ret = (cPtr == IntPtr.Zero) ? null : new SceneNode(cPtr, false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPosition(float x, float y, float z) {
    NativeEngineInteropPINVOKE.SceneNode_setPosition(swigCPtr, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public void setOrientation(float w, float x, float y, float z) {
    NativeEngineInteropPINVOKE.SceneNode_setOrientation(swigCPtr, w, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public void setScale(float x, float y, float z) {
    NativeEngineInteropPINVOKE.SceneNode_setScale(swigCPtr, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

}

}