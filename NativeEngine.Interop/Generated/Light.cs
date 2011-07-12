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

public class Light : MovableObject {
  private HandleRef swigCPtr;

  internal Light(IntPtr cPtr, bool cMemoryOwn) : base(NativeEngineInteropPINVOKE.Light_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(Light obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  public override void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          throw new MethodAccessException("C++ destructor does not have public access");
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
      base.Dispose();
    }
  }

  public void setType(Light.LightTypes type) {
    NativeEngineInteropPINVOKE.Light_setType(swigCPtr, (int)type);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Light.LightTypes getType() {
    Light.LightTypes ret = (Light.LightTypes)NativeEngineInteropPINVOKE.Light_getType(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setDiffuseColour(float red, float green, float blue) {
    NativeEngineInteropPINVOKE.Light_setDiffuseColour(swigCPtr, red, green, blue);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public ColourValue getDiffuseColour() {
    ColourValue ret = new ColourValue(NativeEngineInteropPINVOKE.Light_getDiffuseColour(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setSpecularColour(float red, float green, float blue) {
    NativeEngineInteropPINVOKE.Light_setSpecularColour(swigCPtr, red, green, blue);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public ColourValue getSpecularColour() {
    ColourValue ret = new ColourValue(NativeEngineInteropPINVOKE.Light_getSpecularColour(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setAttenuation(float range, float constant, float linear, float quadratic) {
    NativeEngineInteropPINVOKE.Light_setAttenuation(swigCPtr, range, constant, linear, quadratic);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public float getAttenuationRange() {
    float ret = NativeEngineInteropPINVOKE.Light_getAttenuationRange(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getAttenuationConstant() {
    float ret = NativeEngineInteropPINVOKE.Light_getAttenuationConstant(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getAttenuationLinear() {
    float ret = NativeEngineInteropPINVOKE.Light_getAttenuationLinear(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float getAttenuationQuadric() {
    float ret = NativeEngineInteropPINVOKE.Light_getAttenuationQuadric(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPosition(float x, float y, float z) {
    NativeEngineInteropPINVOKE.Light_setPosition(swigCPtr, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Vector3 getPosition() {
    Vector3 ret = new Vector3(NativeEngineInteropPINVOKE.Light_getPosition(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setDirection(float x, float y, float z) {
    NativeEngineInteropPINVOKE.Light_setDirection(swigCPtr, x, y, z);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public Vector3 getDirection() {
    Vector3 ret = new Vector3(NativeEngineInteropPINVOKE.Light_getDirection(swigCPtr), false);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setSpotlightRange(float innerAngle, float outerAngle, float falloff) {
    NativeEngineInteropPINVOKE.Light_setSpotlightRange(swigCPtr, innerAngle, outerAngle, falloff);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public float getSpotlightInnerAngle() {
    float ret = NativeEngineInteropPINVOKE.Light_getSpotlightInnerAngle(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public float getSpotlightOuterAngle() {
    float ret = NativeEngineInteropPINVOKE.Light_getSpotlightOuterAngle(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
}

  public float getSpotlightFalloff() {
    float ret = NativeEngineInteropPINVOKE.Light_getSpotlightFalloff(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPowerScale(float power) {
    NativeEngineInteropPINVOKE.Light_setPowerScale(swigCPtr, power);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
  }

  public float getPowerScale() {
    float ret = NativeEngineInteropPINVOKE.Light_getPowerScale(swigCPtr);
    if (NativeEngineInteropPINVOKE.SWIGPendingException.Pending) throw NativeEngineInteropPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public enum LightTypes {
    LT_POINT,
    LT_DIRECTIONAL,
    LT_SPOTLIGHT
  }

}

}
