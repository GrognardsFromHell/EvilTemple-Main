#region

using EvilTemple.Runtime;

#endregion

namespace EvilTemple.NativeEngineInterop
{
    /*
     * Converts from native Qt keycodes to EvilTemple key enumeration literals.
     */

    internal static class QtKeyConverter
    {
        public static Keys ConvertKey(int keyCode)
        {
            switch (keyCode)
            {
                case 0x01000004:
                case 0x01000005:
                    return Keys.Enter;
                case 0x01000006:
                    return Keys.Insert;
                case 0x01000007:
                    return Keys.Delete;
                case 0x01000008:
                    return Keys.Pause;
                case 0x01000009:
                    return Keys.Print;
                case 0x0100000b:
                    return Keys.Clear;
                case 0x01000010:
                    return Keys.Home;
                case 0x01000011:
                    return Keys.End;
                case 0x01000012:
                    return Keys.Left;
                case 0x01000013:
                    return Keys.Up;
                case 0x01000014:
                    return Keys.Right;
                case 0x01000015:
                    return Keys.Down;
                case 0x01000016:
                    return Keys.PageUp;
                case 0x01000017:
                    return Keys.PageDown;
                case 0x01000020:
                    return Keys.Shift;
                case 0x01000021:
                    return Keys.Control;
                case 0x01000022:
                    return Keys.Meta;
                case 0x01000023:
                    return Keys.Alt;
                case 0x01001103:
                    return Keys.AltGr;
                case 0x01000024:
                    return Keys.CapsLock;
                case 0x01000025:
                    return Keys.NumLock;
                case 0x01000026:
                    return Keys.ScrollLock;
                case 0x01000030:
                    return Keys.F1;
                case 0x01000031:
                    return Keys.F2;
                case 0x01000032:
                    return Keys.F3;
                case 0x01000033:
                    return Keys.F4;
                case 0x01000034:
                    return Keys.F5;
                case 0x01000035:
                    return Keys.F6;
                case 0x01000036:
                    return Keys.F7;
                case 0x01000037:
                    return Keys.F8;
                case 0x01000038:
                    return Keys.F9;
                case 0x01000039:
                    return Keys.F10;
                case 0x0100003a:
                    return Keys.F11;
                case 0x0100003b:
                    return Keys.F12;
                case 0x20:
                    return Keys.Space;
                case '+':
                    return Keys.Plus;
                case ',':
                    return Keys.Comma;
                case '-':
                    return Keys.Minus;
                case '.':
                    return Keys.Period;
                case '/':
                    return Keys.Slash;
                case '0':
                    return Keys.Digit0;
                case '1':
                    return Keys.Digit1;
                case '2':
                    return Keys.Digit2;
                case '3':
                    return Keys.Digit3;
                case '4':
                    return Keys.Digit4;
                case '5':
                    return Keys.Digit5;
                case '6':
                    return Keys.Digit6;
                case '7':
                    return Keys.Digit7;
                case '8':
                    return Keys.Digit8;
                case '9':
                    return Keys.Digit9;
                case ':':
                    return Keys.Colon;
                case ';':
                    return Keys.Semicolon;
                case '<':
                    return Keys.Less;
                case '=':
                    return Keys.Equal;
                case '>':
                    return Keys.Greater;
                case 'A':
                    return Keys.A;
                case 'B':
                    return Keys.B;
                case 'C':
                    return Keys.C;
                case 'D':
                    return Keys.D;
                case 'E':
                    return Keys.E;
                case 'F':
                    return Keys.F;
                case 'G':
                    return Keys.G;
                case 'H':
                    return Keys.H;
                case 'I':
                    return Keys.I;
                case 'J':
                    return Keys.J;
                case 'K':
                    return Keys.K;
                case 'L':
                    return Keys.L;
                case 'M':
                    return Keys.M;
                case 'N':
                    return Keys.N;
                case 'O':
                    return Keys.O;
                case 'P':
                    return Keys.P;
                case 'Q':
                    return Keys.Q;
                case 'R':
                    return Keys.R;
                case 'S':
                    return Keys.S;
                case 'T':
                    return Keys.T;
                case 'U':
                    return Keys.U;
                case 'V':
                    return Keys.V;
                case 'W':
                    return Keys.W;
                case 'X':
                    return Keys.X;
                case 'Y':
                    return Keys.Y;
                case 'Z':
                    return Keys.Z;
                default:
                    return Keys.Unknown;
            }
        }

        public static KeyModifiers ConvertModifiers(int modifiers)
        {
            KeyModifiers result = 0;

            if ((modifiers & 0x02000000) != 0)
                result |= KeyModifiers.ShiftModifier;

            if ((modifiers & 0x04000000) != 0)
                result |= KeyModifiers.ControlModifier;

            if ((modifiers & 0x08000000) != 0)
                result |= KeyModifiers.AltModifier;

            if ((modifiers & 0x10000000) != 0)
                result |= KeyModifiers.MetaModifier;

            if ((modifiers & 0x20000000) != 0)
                result |= KeyModifiers.KeypadModifier;

            if (result == 0)
                result = KeyModifiers.NoModifier;

            return result;
        }
    }
}