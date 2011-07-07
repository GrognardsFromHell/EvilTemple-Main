using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{

    public class KeyEvent
    {

        public bool IsAutoRepeat { get; private set; }

        public Keys Keys { get; private set; }

        public KeyModifiers Modifiers { get; private set; }

        public string Text { get; private set; }

        public KeyEvent(bool isAutoRepeat, Keys keys, KeyModifiers modifiers, string text)
        {
            IsAutoRepeat = isAutoRepeat;
            Keys = keys;
            Modifiers = modifiers;
            Text = text;
        }

    }
}
