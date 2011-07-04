using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EvilTemple.Runtime;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;

namespace EvilTemple.Support
{
    public class GameModules
    {

        private readonly IList<GameModule> _loadedModules;

        public GameModules()
        {
            _loadedModules = new List<GameModule>();
        }

        /// <summary>
        /// Loads a game module.
        /// </summary>
        /// <param name="path">The filesystem path to the game module archive (a zip file).</param>
        public void LoadModule(string path)
        {
            var manifest = GameModuleManifest.Read(path);
            
            if (manifest == null)
                throw new InvalidDataException("Module " + path + " doesn't have a module manifest.");

            // TODO: Check the module requirements


            // Initialize the module
            var module = new GameModule(manifest, path);
            Services.Kernel.Inject(module);

            EventBus.Send(new StartingModuleMessage(module));

            module.Start();
            _loadedModules.Add(module);

            EventBus.Send(new StartedModuleMessage(module));
        }

        public void LoadModules(IEnumerable<string> modules)
        {
            // TODO: Resolve module dependencies and version requirements instead of simply iterating
            foreach (var modulePath in modules)
                LoadModule(modulePath);
        }


    }

    public class ModuleMessage : IMessage
    {
        public GameModule Module { get; private set; }

        public ModuleMessage(GameModule gameModule)
        {
            Module = gameModule;
        }
    }

    public class StartingModuleMessage : ModuleMessage
    {
        public StartingModuleMessage(GameModule gameModule) : base(gameModule)
        {
        }
    }

    public class StartedModuleMessage : ModuleMessage
    {
        public StartedModuleMessage(GameModule gameModule) : base(gameModule)
        {
        }
    }

}
