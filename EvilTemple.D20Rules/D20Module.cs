using System.IO;
using System.Reflection;
using EvilTemple.Rules;
using EvilTemple.Rules.Feats;
using Newtonsoft.Json;
using Ninject;
using Ninject.Modules;
using Rules;

namespace EvilTemple.D20Rules
{
    public class D20Module : NinjectModule
    {
        public override string Name
        {
            get { return "D20Module"; }
        }

        public override void Load()
        {
            var skills = Kernel.Get<Skills>();
            var stream = GetResource("Skills.json");
            skills.Load(new JsonTextReader(new StreamReader(stream)));
            
            var races = Kernel.Get<Races>();
            stream = GetResource("Races.json");
            races.Load(new JsonTextReader(new StreamReader(stream)));

            var feats = Kernel.Get<FeatRegistry>();
            stream = GetResource("Feats.json");
            feats.Load(new JsonTextReader(new StreamReader(stream)));

            var domains = Kernel.Get<Domains>();
            stream = GetResource("Domains.json");
            domains.Load(new JsonTextReader(new StreamReader(stream)));

            var deities = Kernel.Get<Deities>();
            stream = GetResource("Deities.json");
            deities.Load(new JsonTextReader(new StreamReader(stream)));

            var progressionTables = Kernel.Get<ProgressionTables>();
            stream = GetResource("ProgressionTables.json");
            progressionTables.Load(new JsonTextReader(new StreamReader(stream)));

            var bonusFeats = Kernel.Get<BonusFeatLists>();
            stream = GetResource("BonusFeatLists.json");
            bonusFeats.Load(new JsonTextReader(new StreamReader(stream)));

            var classes = Kernel.Get<CharacterClasses>();
            stream = GetResource("Classes.json");
            classes.Load(new JsonTextReader(new StreamReader(stream)));
        }

        private static Stream GetResource(string filename)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var stream = executingAssembly.GetManifestResourceStream(typeof(D20Module), filename);
            if (stream == null)
                throw new FileNotFoundException("Unable to find resource " + filename + " in assembly " + executingAssembly.Location);
            return stream;
        }
    }
}
