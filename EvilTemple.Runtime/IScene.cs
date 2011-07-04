using System.Collections.Generic;
using OpenTK;

namespace EvilTemple.Runtime
{

    public interface IScene
    {
        int ObjectsDrawn { get; }

        ISceneNode CreateNode();

        void Add(ISceneNode node);

        void Remove(ISceneNode node);

        void Clear();

        void AddOverlayText(ref Vector4 position, string text, ref Vector4 color, float lifetime);
    }

    public interface ISceneNode
    {

        Vector3 Position { get; set; }

        Vector3 Scale { get; set; }

        Quaternion Rotation { get; set; }

        bool Interactive { get; set; }

        ISceneNode Parent { get; set; }

        IList<IRenderable> AttachedObjects { get; }

        void Attach(IRenderable renderable);

        void Detach(IRenderable renderable);

    }
}
