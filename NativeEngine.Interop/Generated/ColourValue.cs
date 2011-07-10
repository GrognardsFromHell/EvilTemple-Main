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

public class ColourValue : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ColourValue(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(ColourValue obj) {
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

  public float r {
    set {
      NativeEngineInteropPINVOKE.ColourValue_r_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativeEngineInteropPINVOKE.ColourValue_r_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public float g {
    set {
      NativeEngineInteropPINVOKE.ColourValue_g_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativeEngineInteropPINVOKE.ColourValue_g_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public float b {
    set {
      NativeEngineInteropPINVOKE.ColourValue_b_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativeEngineInteropPINVOKE.ColourValue_b_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public float a {
    set {
      NativeEngineInteropPINVOKE.ColourValue_a_set(swigCPtr, value);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      float ret = NativeEngineInteropPINVOKE.ColourValue_a_get(swigCPtr);
      if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

}

}
