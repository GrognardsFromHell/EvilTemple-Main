using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EvilTemple.D20Rules;
using EvilTemple.NativeEngineInterop;
using EvilTemple.NativeEngineInterop.Generated;
using EvilTemple.Rules;
using EvilTemple.Runtime.Messages;
using EvilTemple.Support;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Ninject;
using OpenTK;
using Quaternion = EvilTemple.NativeEngineInterop.Generated.Quaternion;
using Vector3 = EvilTemple.NativeEngineInterop.Generated.Vector3;

namespace EvilTemple
{
    internal static class Startup
    {
        private delegate void LogListener([MarshalAs(UnmanagedType.LPStr)] string name, 
            [MarshalAs(UnmanagedType.LPStr)] string message, 
            int level, 
            [MarshalAs(UnmanagedType.Bool)] bool debug);

        private static void LogMessage(string logName, string message, int level, bool debug)
        {
            Console.WriteLine("[" + level + "]: " + message);
        }

        private static readonly LogListener LogListenerDelegate = LogMessage;

        [STAThread]
        public static void Main(string[] args)
        {
            var paths = new Paths();
            var shortcuts = new Shortcuts();
            
            /*
            InitializeConsoleLogging();
            InitializeFileLogging();

            kernel.Bind<IResourceManager>().ToConstant(resourceManager);
            kernel.Bind<IShortcuts>().ToConstant(shortcuts);

            LoadResources(resourceManager);
             * */

            var activeAnimations = new List<string>();
            
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

                var kernel = new StandardKernel();
                kernel.Bind<IResourceManager>().ToConstant(new ResourceManagerAdapter());
                Services.Kernel = kernel;

                kernel.Load(new RulesModule(), new D20Module(), new SupportModule());

                LoadModules(kernel);

                EventBus.Send<ApplicationStartup>();

                ResourceManager.initializeGroup("General");

                int w = engine.windowWidth();
                int h = engine.windowHeight();

                var lastObjectId = 0;
                var selectableObjects = new Dictionary<long, BaseObject>();

                engine.OnKeyPress += e =>
                                         {
                                             if (e.Keys == Keys.Right)
                                             {
                                                 var cam = engine.mainScene().GetMainCamera();
                                                 using (var v3 = new Vector3(10, 0, 0))
                                                     cam.Move(v3);
                                             }

                                             Console.WriteLine("KEYPRESS: " + e.Text);
                                         };
                engine.OnKeyRelease += e => Console.WriteLine("KEYRELEASE: " + e.Text);
                engine.OnMousePress += e => Console.WriteLine("Mouse Press: " + e.X + "," + e.Y);
                engine.OnMouseRelease += e => Console.WriteLine("Mouse Release " + e.X + "," + e.Y);
                engine.OnMouseMove += e =>{
                                              var x = e.X / (float)w;
                                              var y = e.Y / (float)h;

                                              using (var pickResultList = engine.mainScene().pick(x, y))
                                              {
                                                  var count = pickResultList.size();
                                                  for (var i = 0; i < count; ++i)
                                                  {
                                                      using (var pickResult = pickResultList.at(i))
                                                      {
                                                          var obj = selectableObjects[pickResult.id];
                                                          Console.WriteLine("MouseOver: " + obj.Model);
                                                      }
                                                  }
                                              }
                };
                engine.OnMouseDoubleClick += e => Console.WriteLine("Mouse Double Click " + e.X + "," + e.Y);

                var staticObjectsJson = ResourceManager.ReadFile(@"maps/map-2-hommlet-exterior/staticObjects.json");

                var reader = new JsonTextReader(new StringReader(Encoding.UTF8.GetString(staticObjectsJson)));
                var objects = BaseObjectSerializer.Serializer.Deserialize<List<BaseObject>>(reader);

                var scene = engine.mainScene();

                const float PixelPerWorldTile = 28.2842703f;
                {
                    var entity = scene.CreateEntity("meshes/pcs/pc_human_male/pc_human_male.mesh");
                    var sceneNode = scene.GetRootSceneNode().createChildSceneNode();
                    sceneNode.attachObject(entity);
                    sceneNode.setPosition(PixelPerWorldTile * 480, 0, -PixelPerWorldTile * 480);
                }

                foreach (var obj in objects)
                {

                    var entity = scene.CreateEntity(obj.Model);

                    var sceneNode = scene.GetRootSceneNode().createChildSceneNode();
                    sceneNode.setPosition(obj.Position.X, obj.Position.Y, -obj.Position.Z);
                    sceneNode.attachObject(entity);

                    if (obj.Interactive)
                    {
                        var selectionId = ++lastObjectId;
                        selectableObjects[selectionId] = obj;
                        entity.setSelectionData(selectionId, obj.SelectionRadius, obj.SelectionHeight);

                        var selectionChildNode = sceneNode.createChildSceneNode();
                        var selectionCircle = scene.CreateGroundDisc("meshes/mouseoverenemy");
                        selectionChildNode.attachObject(selectionCircle);
                        selectionChildNode.setScale(obj.SelectionRadius, obj.SelectionRadius, obj.SelectionRadius);
                        selectionChildNode.setInitialState();

                        var selectionAnim = scene.CreateAnimation("SelectionAnim-" + selectionId, 5);
                        var animTrack = selectionAnim.createNodeTrack(0, selectionChildNode);
                        var tnode = animTrack.createNodeKeyFrame(0);
                        using (var rot = new Quaternion(1, 0, 0, 0))
                            tnode.setRotation(rot);

                        tnode = animTrack.createNodeKeyFrame(1.25f);
                        using (var rot = new Quaternion())
                        using (var axis = new Vector3(0, 1, 0))
                        {
                            rot.FromAngleAxis(MathHelper.DegreesToRadians(90), axis);
                            tnode.setRotation(rot);
                        }

                        tnode = animTrack.createNodeKeyFrame(2.5f);
                        using (var rot = new Quaternion())
                        using (var axis = new Vector3(0, 1, 0))
                        {
                            rot.FromAngleAxis(MathHelper.DegreesToRadians(180), axis);
                            tnode.setRotation(rot);
                        }

                        tnode = animTrack.createNodeKeyFrame(3.75f);
                        using (var rot = new Quaternion())
                        using (var axis = new Vector3(0, 1, 0))
                        {
                            rot.FromAngleAxis(MathHelper.DegreesToRadians(270), axis);
                            tnode.setRotation(rot);
                        }

                        tnode = animTrack.createNodeKeyFrame(5f);
                        using (var rot = new Quaternion())
                        using (var vector = new Vector3(0, 1, 0))
                        {
                            rot.FromAngleAxis(MathHelper.DegreesToRadians(360), vector);
                            tnode.setRotation(rot);
                        }

                        var state = scene.CreateAnimationState("SelectionAnim-" + selectionId);
                        state.setEnabled(true);
                        state.setLoop(true);

                        activeAnimations.Add("SelectionAnim-" + selectionId);
                    }
                }

                var light = scene.CreateLight();
                light.setType(Light.LightTypes.LT_DIRECTIONAL);
                light.setDirection(-0.6324093645670703858428703903848f,
                    -0.77463436252716949786709498111783f,
                    0f);
                scene.GetRootSceneNode().attachObject(light);

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
                var sw = new Stopwatch();
                sw.Start();
                var msPerFrame = 1000/60;
                
                while (true)
                {
                    if (sw.ElapsedMilliseconds > msPerFrame)
                    {
                        var elapsed = sw.ElapsedMilliseconds/1000.0f;
                        sw.Restart();
                        foreach (var activeAnimId in activeAnimations)
                        {
                            var animState = scene.getAnimationState(activeAnimId);
                            animState.addTime(elapsed);
                        }
                    }

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
            var settings = new NativeEngineSettings();

            settings.logCallback = Marshal.GetFunctionPointerForDelegate(LogListenerDelegate);

            return settings;
        }

        private static void LoadModules(IKernel kernel)
        {
            var loader = kernel.Get<GameModuleLoader>();
            loader.LoadAllModules();
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