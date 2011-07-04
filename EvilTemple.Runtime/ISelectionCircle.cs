using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace EvilTemple.Runtime
{
    public interface ISelectionCircle : IRenderable
    {

        float Radius { get; set; }

        float Height { get; set; }

        Vector4 Color { get; set; }

        bool Selected { get; set; }

        bool Hovering { get; set; }

    }
}
