#region

using System;
using System.Runtime.InteropServices;
using EvilTemple.Runtime;

#endregion

namespace EvilTemple.NativeEngineInterop.Generated
{

    internal delegate void NativeKeyEventHandler(
        int count, 
        [MarshalAs(UnmanagedType.Bool)] bool isAutoRepeat, 
        int key, 
        int modifiers,
        [MarshalAs(UnmanagedType.LPWStr)] string text);

    /* TODO: Check if pack is necessary */

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    internal struct NativeMouseEvent
    {
        public int button;
        public int buttons;
        public int x;
        public int y;
    }

    internal delegate void NativeMouseEventHandler(ref NativeMouseEvent e);

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeWheelEvent
    {
        public int x;
        public int y;
        public int buttons;
        public int delta;
        public int orientation;
    }

    internal delegate void NativeWheelEventHandler(NativeWheelEvent e);

    public partial class NativeEngine
    {
        private Action<KeyEvent> _keyPressEvent;

        private Action<KeyEvent> _keyReleaseEvent;

        private Action<MouseEvent> _mouseDoubleClickEvent;

        private Action<MouseEvent> _mousePressEvent;

        private Action<MouseEvent> _mouseReleaseEvent;

        private Action<MouseEvent> _mouseMoveEvent;
        
        private NativeKeyEventHandler _keyPressHandler;

        private NativeKeyEventHandler _keyReleaseHandler;

        private NativeMouseEventHandler _mouseDoubleClickHandler;

        private NativeMouseEventHandler _mousePressHandler;

        private NativeMouseEventHandler _mouseReleaseHandler;

        private NativeMouseEventHandler _mouseMoveHandler;

        private NativeMouseEventHandler _wheelHandler;

        private void OnKeyPressHandler(int c, bool ap, int key, int modifiers, string text)
        {
            if (_keyPressEvent != null)
            {
                var keyEvent = new KeyEvent(ap, QtKeyConverter.ConvertKey(key), QtKeyConverter.ConvertModifiers(modifiers), text);

                _keyPressEvent(keyEvent);
            }
        }

        private void OnKeyReleaseHandler(int c, bool ap, int key, int modifiers, string text)
        {
            if (_keyReleaseEvent != null)
            {
                var keyEvent = new KeyEvent(ap, QtKeyConverter.ConvertKey(key), QtKeyConverter.ConvertModifiers(modifiers), text);

                _keyReleaseEvent(keyEvent);
            }
        }

        private static MouseEvent CreateMouseEvent(NativeMouseEvent nativeEvent)
        {
            var result = new MouseEvent();
            switch (nativeEvent.button)
            {
                case 1:
                    result.Button = MouseButton.LeftButton;
                    break;
                case 2:
                    result.Button = MouseButton.RightButton;
                    break;
                case 4:
                    result.Button = MouseButton.MiddleButton;
                    break;
                case 8:
                    result.Button = MouseButton.XButton1;
                    break;
                case 16:
                    result.Button = MouseButton.XButton2;
                    break;
            }

            if ((nativeEvent.buttons & 1) == 1)
                result.Buttons |= MouseButtons.LeftButton;

            if ((nativeEvent.buttons & 2) == 2)
                result.Buttons |= MouseButtons.RightButton;

            if ((nativeEvent.buttons & 4) == 4)
                result.Buttons |= MouseButtons.MiddleButton;

            if ((nativeEvent.buttons & 8) == 8)
                result.Buttons |= MouseButtons.XButton1;

            if ((nativeEvent.buttons & 16) == 16)
                result.Buttons |= MouseButtons.XButton2;

            result.X = nativeEvent.x;
            result.Y = nativeEvent.y;

            return result;
        }

        private void OnMouseDoubleClickHandler(ref NativeMouseEvent nativeEvent)
        {
            if (_mouseDoubleClickEvent != null)
            {
                var mouseEvent = CreateMouseEvent(nativeEvent);

                _mouseDoubleClickEvent(mouseEvent);
            }
        }

        private void OnMousePressHandler(ref NativeMouseEvent nativeEvent)
        {
            if (_mousePressEvent != null)
            {
                var mouseEvent = CreateMouseEvent(nativeEvent);

                _mousePressEvent(mouseEvent);
            }
        }

        private void OnMouseReleaseHandler(ref NativeMouseEvent nativeEvent)
        {
            if (_mouseReleaseEvent != null)
            {
                var mouseEvent = CreateMouseEvent(nativeEvent);

                _mouseReleaseEvent(mouseEvent);
            }
        }

        private void OnMouseMoveHandler(ref NativeMouseEvent nativeEvent)
        {
            if (_mouseMoveEvent != null)
            {
                var mouseEvent = CreateMouseEvent(nativeEvent);

                _mouseMoveEvent(mouseEvent);
            }
        }

        public event Action<KeyEvent> OnKeyPress
        {
            add
            {
                _keyPressEvent = value;
                if (_keyPressHandler == null)
                    _keyPressHandler = OnKeyPressHandler;
                NativeEngine_SetKeyPressCallback(swigCPtr.Handle, _keyPressHandler);
            }
            remove
            {
                _keyPressEvent = null;
                NativeEngine_SetKeyPressCallback(swigCPtr.Handle, null);
            }
        }

        public event Action<KeyEvent> OnKeyRelease
        {
            add
            {
                _keyReleaseEvent = value;
                if (_keyReleaseHandler == null)
                    _keyReleaseHandler = OnKeyReleaseHandler;
                NativeEngine_SetKeyReleaseCallback(swigCPtr.Handle, _keyReleaseHandler);
            }
            remove
            {
                _keyReleaseHandler = null;
                NativeEngine_SetKeyReleaseCallback(swigCPtr.Handle, null);
            }
        }

        public event Action<MouseEvent> OnMouseDoubleClick
        {
            add
            {
                _mouseDoubleClickEvent = value;
                if (_mouseDoubleClickHandler == null)
                    _mouseDoubleClickHandler = OnMouseDoubleClickHandler;
                NativeEngine_SetMouseDoubleClickCallback(swigCPtr.Handle, _mouseDoubleClickHandler);
            }
            remove
            {
                _mouseDoubleClickEvent = null;
                NativeEngine_SetMouseDoubleClickCallback(swigCPtr.Handle, null);
            }
        }

        public event Action<MouseEvent> OnMousePress
        {
            add
            {
                _mousePressEvent = value;
                if (_mousePressHandler == null)
                    _mousePressHandler = OnMousePressHandler;
                NativeEngine_SetMousePressCallback(swigCPtr.Handle, _mousePressHandler);
            }
            remove
            {
                _mousePressEvent = null;
                NativeEngine_SetMousePressCallback(swigCPtr.Handle, null);
            }
        }

        public event Action<MouseEvent> OnMouseRelease
        {
            add
            {
                _mouseReleaseEvent = value;
                if (_mouseReleaseHandler == null)
                    _mouseReleaseHandler = OnMouseReleaseHandler;
                NativeEngine_SetMouseReleaseCallback(swigCPtr.Handle, _mouseReleaseHandler);
            }
            remove
            {
                _mouseReleaseEvent = null;
                NativeEngine_SetMouseReleaseCallback(swigCPtr.Handle, null);
            }
        }

        public event Action<MouseEvent> OnMouseMove
        {
            add
            {
                _mouseMoveEvent = value;
                if (_mouseMoveHandler == null)
                    _mouseMoveHandler = OnMouseMoveHandler;
                NativeEngine_SetMouseMoveCallback(swigCPtr.Handle, _mouseMoveHandler);
            }
            remove
            {
                _mouseMoveEvent = null;
                NativeEngine_SetMouseMoveCallback(swigCPtr.Handle, null);
            }
        }
       
        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetKeyPressCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeKeyEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetKeyReleaseCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeKeyEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetMouseDoubleClickCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeMouseEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetMousePressCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeMouseEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetMouseReleaseCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeMouseEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetMouseMoveCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeMouseEventHandler handler);

        [DllImport("NativeEngine")]
        private static extern void NativeEngine_SetWheelCallback(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.FunctionPtr)] NativeWheelEvent handler);
    }
}