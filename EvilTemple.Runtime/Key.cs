﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    public enum Keys
    {
        Unknown,
        Return,
        Enter,
        Insert,
        Delete,
        Pause,
        Print,
        Clear,
        Home,
        End,
        Left,
        Up,
        Right,
        Down,
        PageUp,
        PageDown,
        Shift,
        Control,
        Meta,
        Alt,
        AltGr,
        CapsLock,
        NumLock,
        ScrollLock,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        Space,
        Any,
        Plus,
        Comma,
        Minus,
        Period,
        Slash,
        Digit0,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,
        Colon,
        Semicolon,
        Less,
        Equal,
        Greater,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z
    }

    [Flags]
    public enum KeyModifiers
    {
        NoModifier,
        ShiftModifier,
        ControlModifier,
        AltModifier,
        MetaModifier,
        KeypadModifier
    }

}