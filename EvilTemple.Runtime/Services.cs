using System;
using Ninject;

namespace EvilTemple.Runtime
{
    public static class Services
    {

        public static IKernel Kernel { get; set; }

        public static TService Get<TService>()
        {
            return Kernel.Get<TService>();
        }
        
        public static IModels Models
        {
            get { return Kernel.Get<IModels>(); }
        }

        public static IGameView GameView
        {
            get { return Kernel.Get<IGameView>(); }
        }

        public static IResourceManager ResourceManager
        {
            get { return Kernel.Get<IResourceManager>(); }
        }

        public static IUserSettings UserSettings
        {
            get { return Kernel.Get<IUserSettings>(); }
        }

        public static IScene Scene
        {
            get { return Kernel.Get<IScene>(); }
        }

        public static IRenderableFactory RenderableFactory
        {
            get { return Kernel.Get<IRenderableFactory>(); }
        }

        public static IMaterials Materials
        {
            get { return Get<IMaterials>(); }
        }

        public static IShortcuts Shortcuts
        {
            get { return Get<IShortcuts>(); }
        }
    }
}
