using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EngineInterop;
using EvilTemple.D20Rules;
using EvilTemple.Gui;
using EvilTemple.Rules;
using EvilTemple.Support;
using Ninject;
using Ninject.Syntax;
using EvilTemple.Runtime;
using EvilTemple.Runtime.Messages;
using Rules;

namespace EvilTemple
{
    internal static class Startup
    {
        [STAThread]
        public static void Main(string[] args)
        {
            InitializeConsoleLogging();
            InitializeFileLogging();

            var shortcuts = new Shortcuts();

            LeakedObjectMonitor.Install();
            
            using (var resourceManager = new ResourceManager())
            {
                var kernel = new StandardKernel();
                Services.Kernel = kernel;

                kernel.Bind<IResourceManager>().ToConstant(resourceManager);
                kernel.Bind<IShortcuts>().ToConstant(shortcuts);

                LoadResources(resourceManager);
                
                /*
                 * There is one massive problem with this:
                 * If the GC's finalizer thread gets to collect this object, it will destroy the
                 * QApplication, which in turn will send events to some internal widgets, although 
                 * they have been created on a different thread. This leads to a shutdown crash.
                 */
                using (var engine = new Engine(args))
                {
                    LoadFonts(engine);

                    // Subscribe to the events the engine provides
                    engine.OnKeyPress += shortcuts.HandleEvent;
                    engine.OnDrawFrame += EventBus.Send<DrawFrameMessage>;
                    
                    // Add several objects provided only by the engine
                    AddEngineObjects(kernel, engine);

                    // Initialize all the other modules that depend on the engine
                    kernel.Load(new RulesModule(), new D20Module(), new SupportModule(), new GuiModule());

                    // Load Modules
                    LoadModules(kernel);

                    EventBus.Send<ApplicationStartup>();

                    // Run the engine main loop
                    engine.Run();

                    EventBus.Send<ApplicationShutdown>();

                    /* Try to release all references to game data before shutting down the engine */
                    Services.Kernel = null;
                    kernel.Dispose();
                    GC.Collect();
                }
            }
        }
        
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

        private static void LoadFonts(Engine engine)
        {
            engine.LoadFont(@"fonts\5inq_-_Handserif.ttf");
            engine.LoadFont(@"fonts\ArtNoveauDecadente.ttf");
            engine.LoadFont(@"fonts\Fontin-Bold.ttf");
            engine.LoadFont(@"fonts\Fontin-Italic.ttf");
            engine.LoadFont(@"fonts\Fontin-Regular.ttf");
            engine.LoadFont(@"fonts\Fontin-SmallCaps.ttf");
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
        }
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