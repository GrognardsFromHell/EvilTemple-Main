using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{

    public enum MouseButton
    {
        NoButton,
        LeftButton,
        RightButton,
        MiddleButton,
        XButton1,
        XButton2
    }

    [Flags]
    public enum MouseButtons
    {
        LeftButton,
        RightButton,
        MiddleButton,
        XButton1,
        XButton2
    }

    public class MouseEvent
    {

        public int X { get; set; }

        public int Y { get; set; }

        public MouseButton Button { get; set; }

        public MouseButtons Buttons { get; set; }

    }
}
