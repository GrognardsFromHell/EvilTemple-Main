using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Ninject;
using Rules;

namespace EvilTemple.Support
{
    public class GameModule : IIdentifiable
    {

        public string Id
        {
            get { return Manifest.Id; }
        }

        public GameModuleManifest Manifest { get; private set; }

        public string Path { get; private set; }

        [Inject]
        public IResourceManager ResourceManager { get; set; }

        public GameModule(GameModuleManifest manifest, string path)
        {
            Manifest = manifest;
            Path = path;
        }

        public void Start()
        {
            Trace.TraceInformation("Starting module {0}", Path);

            MountArchives();

            LoadResources();
        }

        protected virtual void MountArchives()
        {
            ResourceManager.MountArchive(Path);
        }

        protected virtual void LoadResources()
        {
            foreach (var mapFile in Manifest.Maps)
            {
                var stream = new MemoryStream(ResourceManager.ReadResource(mapFile));
                Services.Get<Maps>().LoadMap(stream);
            }

            LoadResources(Manifest.Voices, Services.Get<PlayerVoices>());
            LoadResources(Manifest.JumpPoints, Services.Get<JumpPoints>());
            LoadResources(Manifest.Portraits, Services.Get<Portraits>());
            LoadResources(Manifest.InventoryIcons, Services.Get<InventoryIcons>());
            LoadResources(Manifest.Equipment, Services.Get<EquipmentStyles>());
            LoadResources(Manifest.HairStyles, Services.Get<HairStyles>());
            LoadResources(Manifest.Prototypes, Services.Get<Prototypes>());
            LoadTranslations();
        }

        private void LoadTranslations()
        {
            var translations = Services.Get<Translations>();
            var resources = Services.ResourceManager;

            foreach (var t in Manifest.Translations)
            {
                var content = resources.ReadResource(t);
                var stream = new MemoryStream(content);

                translations.Load(stream);
            }
        }

        protected virtual void LoadResources<T>(IEnumerable<string> files, Registry<T> registry) where T : class, IIdentifiable
        {
            var resources = Services.ResourceManager;

            foreach (var path in files)
            {
                var file = resources.ReadResource(path);

                var stream = new MemoryStream(file);

                registry.Load(new JsonTextReader(new StreamReader(stream, Encoding.UTF8)));
            }            
        }
    }
}
