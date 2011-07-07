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

public class NativeEngineSettings : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal NativeEngineSettings(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(NativeEngineSettings obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~NativeEngineSettings() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          NativeEngineInteropPINVOKE.delete_NativeEngineSettings(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public int argc {
    set {
      NativeEngineInteropPINVOKE.NativeEngineSettings_argc_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      int ret = NativeEngineInteropPINVOKE.NativeEngineSettings_argc_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public SWIGTYPE_p_p_char argv {
    set {
      NativeEngineInteropPINVOKE.NativeEngineSettings_argv_set(swigCPtr, SWIGTYPE_p_p_char.getCPtr(value));
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      IntPtr cPtr = NativeEngineInteropPINVOKE.NativeEngineSettings_argv_get(swigCPtr);
      SWIGTYPE_p_p_char ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_p_char(cPtr, false);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public NativeEngineSettings() : this(NativeEngineInteropPINVOKE.new_NativeEngineSettings(), true) {
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
