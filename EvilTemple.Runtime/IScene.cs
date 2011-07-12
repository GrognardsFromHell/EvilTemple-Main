using System.Collections.Generic;
using OpenTK;

namespace EvilTemple.Runtime
{

    public interface IScene
    {
        ISceneNode CreateNode();

        void Add(ISceneNode node);

        void Remove(ISceneNode node);

        void Clear();
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
