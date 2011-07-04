using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace EvilTemple.Runtime
{
    public interface IModelInstance : IRenderable
    {
        IModel Model { get; set; }

        bool Idling { get; }

        string IdleAnimation { get; set; }

        bool DrawBehindWalls { get; set; }

        void AddMesh(IModel model);

        bool OverrideMaterial(string name, IMaterialState material);

        bool ClearOverrideMaterial(string name);

        void ClearOverrideMaterials();

        void SetMaterialProperty(string name, float value);

        void SetMaterialProperty(string name, Vector4 value);

        bool PlayAnimation(string name, bool loop = false);

        void StopAnimation();

        void ElapseTime(float time);

        void ElapseDistance(float distance);

        void ElapseRotation(float rotation);

        event Action<string, bool> OnAnimationFinished;

        event Action<AnimationEvent> OnAnimationEvent;
    }

    public interface IMaterialState
    {
    }

    public interface IMaterials
    {
        IMaterialState Load(string filename);
    }

}
