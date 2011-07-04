using OpenTK;

namespace EvilTemple.Runtime
{
    public interface IBackgroundMap : IRenderable
    {
        string Directory { get; set; }

        Vector4 Color { get; set; }

    }
}