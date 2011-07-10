using System;
using System.Collections.Generic;
using System.IO;
using EvilTemple.NativeEngineInterop;
using EvilTemple.Support;
using EvilTemple.Runtime;

namespace EvilTemple
{
    internal static class Startup
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var paths = new Paths();
            var shortcuts = new Shortcuts();
            /*
            InitializeConsoleLogging();
            InitializeFileLogging();

            

            var kernel = new StandardKernel();
            Services.Kernel = kernel;

            kernel.Bind<IResourceManager>().ToConstant(resourceManager);
            kernel.Bind<IShortcuts>().ToConstant(shortcuts);

            LoadResources(resourceManager);
             * */

            /*
                * There is one massive problem with this:
                * If the GC's finalizer thread gets to collect this object, it will destroy the
                * QApplication, which in turn will send events to some internal widgets, although 
                * they have been created on a different thread. This leads to a shutdown crash.
                */
            using (var engineSettings = CreateEngineSettings())
            using (var engine = new NativeEngine(engineSettings))
            {
                // Initialize resources
                ResourceManager.addZipArchive("General", Path.Combine(paths.GeneratedDataPath, "data.zip"));
                ResourceManager.addDirectory("General", Path.Combine(paths.InstallationPath, "data"));
                ResourceManager.addDirectory("General", Path.Combine(paths.GeneratedDataPath, "override"));
                ResourceManager.initializeGroup("General");

                engine.OnKeyPress += e => Console.WriteLine("KEYPRESS: " + e.Text);
                engine.OnKeyRelease += e => Console.WriteLine("KEYRELEASE: " + e.Text);
                engine.OnMousePress += e => Console.WriteLine("Mouse Press: " + e.X + "," + e.Y);
                engine.OnMouseRelease += e => Console.WriteLine("Mouse Release " + e.X + "," + e.Y);
                engine.OnMouseMove += e => Console.WriteLine("Mouse Move " + e.X + "," + e.Y);
                engine.OnMouseDoubleClick += e => Console.WriteLine("Mouse Double Click " + e.X + "," + e.Y);

                var scene = engine.mainScene();

                var entity = scene.CreateEntity("meshes/pcs/pc_human_male/pc_human_male.mesh");
                var sceneNode = scene.GetRootSceneNode().CreateChildSceneNode();
                sceneNode.AttachObject(entity);

                var light = scene.CreateLight();
                light.setType(Light.LightTypes.LT_DIRECTIONAL);
                light.setDirection(-0.6324093645670703858428703903848f,
                    -0.77463436252716949786709498111783f,
                    0f);
                sceneNode.AttachObject(light);
                const float PixelPerWorldTile = 28.2842703f;

                sceneNode.setPosition(PixelPerWorldTile * 480, 0, -PixelPerWorldTile * 480);

                var camera = scene.GetMainCamera();
                camera.Move(new Vector3(PixelPerWorldTile * 480, 0, -PixelPerWorldTile * 480));

                var background = scene.CreateBackgroundMap("backgroundmaps/map-2-hommlet-exterior");

                // Subscribe to the events the engine provides
                //engine.OnKeyPress += shortcuts.HandleEvent;
                //engine.OnDrawFrame += EventBus.Send<DrawFrameMessage>;
                    
                // Add several objects provided only by the engine
                //AddEngineObjects(kernel, engine);

                // Initialize all the other modules that depend on the engine
                //kernel.Load(new RulesModule(), new D20Module(), new SupportModule(), new GuiModule());

                // Load Modules
                //LoadModules(kernel);

                //EventBus.Send<ApplicationStartup>();

                // Run the engine main loop););));));););
                while (true)
                {
                    engine.processEvents();
                    engine.renderFrame();
                }

                /*
                EventBus.Send<ApplicationShutdown>();
                */
                /* Try to release all references to game data before shutting down the engine */
                /*Services.Kernel = null;
                kernel.Dispose();
                GC.Collect();*/
            }
        }

        private static NativeEngineSettings CreateEngineSettings()
        {
            return new NativeEngineSettings();
        }

        /*
        private static void InitializeFileLogging()
        {
            var paths = new Paths();
            var logFilePath = Path.Combine(paths.UserDataPath, "Logs");
            Directory.CreateDirectory(logFilePath);

            logFilePath = Path.Combine(logFilePath, "eviltemple.log");

            Trace.TraceInformation("Opening log file {0}", logFilePath);

            File.Delete(logFilePath);

            var listener = new TextWriterTraceListener(logFilePath);
            Trace.Listeners.Add(listener);
        }

        private static void InitializeConsoleLogging()
        {
            var listener = new ColoredTraceListener();
            Trace.Listeners.Add(listener);
            SystemMessages.ConvertToTrace = true;
        }

        private static void LoadModules(IKernel kernel)
        {
            var loader = kernel.Get<GameModuleLoader>();
            loader.LoadAllModules();
        }

        private static void LoadResources(ResourceManager resourceManager)
        {
            var paths = new Paths();

            resourceManager.OverrideDataPath = Path.Combine(paths.InstallationPath, "data");
        }

        private static void AddEngineObjects(IBindingRoot kernel, Engine engine)
        {
            kernel.Bind<IGameView>().ToConstant(engine.GameView);
            kernel.Bind<IScene>().ToConstant(engine.Scene);
            kernel.Bind<IModels>().ToConstant(engine.Models);
            kernel.Bind<IMaterials>().ToConstant(engine.Materials);
            kernel.Bind<IAudioEngine>().ToConstant(engine.AudioEngine);
            kernel.Bind<IUserInterface>().ToConstant(engine.UserInterface);

            kernel.Bind<IRenderableFactory>().ToConstant(engine.RenderableFactory);
        }*/
    }

    class Shortcuts : IShortcuts
    {

        private Dictionary<Keys, Action> _callbacks = new Dictionary<Keys, Action>();

        public void Register(Keys key, Action callback)
        {
            _callbacks[key] = callback;
        }

        public void HandleEvent(Keys key, KeyModifiers modifiers, string text)
        {
            if (_callbacks.ContainsKey(key))
                _callbacks[key]();
        }

    }

}