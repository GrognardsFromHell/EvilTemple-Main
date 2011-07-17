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

public class PickResult : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PickResult(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(PickResult obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~PickResult() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          NativeEngineInteropPINVOKE.delete_PickResult(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public long id {
    set {
      NativeEngineInteropPINVOKE.PickResult_id_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      long ret = NativeEngineInteropPINVOKE.PickResult_id_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public float distance {
    set {
      NativeEngineInteropPINVOKE.PickResult_distance_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativeEngineInteropPINVOKE.PickResult_distance_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public PickResult() : this(NativeEngineInteropPINVOKE.new_PickResult(), true) {
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
