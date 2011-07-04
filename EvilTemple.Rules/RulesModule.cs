#region

using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Ninject.Modules;
using Rules;

#endregion

namespace EvilTemple.Rules
{
    public class RulesModule : NinjectModule
    {
        public override string Name
        {
            get { return "Rules"; }
        }

        public override void Load()
        {
            Bind<Races>().ToSelf().InSingletonScope();
            Bind<FeatRegistry>().ToSelf().InSingletonScope();
            Bind<Prototypes.Prototypes>().ToSelf().InSingletonScope();
            Bind<EquipmentStyles>().ToSelf().InSingletonScope();
            Bind<InventoryIcons>().ToSelf().InSingletonScope();
            Bind<Portraits>().ToSelf().InSingletonScope();
            Bind<HairStyles>().ToSelf().InSingletonScope();
            Bind<CharacterClasses>().ToSelf().InSingletonScope();
            Bind<ProgressionTables>().ToSelf().InSingletonScope();
            Bind<BonusFeatLists>().ToSelf().InSingletonScope();
            Bind<JumpPoints>().ToSelf().InSingletonScope();
            Bind<Skills>().ToSelf().InSingletonScope();
            Bind<Domains>().ToSelf().InSingletonScope();
            Bind<Deities>().ToSelf().InSingletonScope();
            Bind<PlayerVoices>().ToSelf().InSingletonScope();
            Bind<Maps>().ToSelf().InSingletonScope();

            var translations = new Translations();
            Bind<ITranslations>().ToConstant(translations);
            Bind<Translations>().ToConstant(translations);
        }
    }

}