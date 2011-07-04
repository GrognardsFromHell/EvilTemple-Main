using System;

namespace EvilTemple.Runtime
{
    public interface IRenderable : IDisposable
    {
        event Action OnMousePressed;
        event Action OnMouseReleased;
        event Action OnDoubleClicked;
        event Action OnMouseEnter;
        event Action OnMouseLeave;
        event Action OnMouseMove;
    }
}