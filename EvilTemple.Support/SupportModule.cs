using EvilTemple.Runtime;
using Ninject.Modules;

namespace EvilTemple.Support
{

    public class SupportModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CharacterVault>().ToSelf().InSingletonScope();
            Bind<GameModules>().ToSelf().InSingletonScope();
            Bind<IPaths>().To(typeof(Paths)).InSingletonScope();
            Bind<IUserSettings>().To(typeof (UserSettings)).InSingletonScope();
            Bind<IScripting>().To(typeof (Scripting)).InSingletonScope();
        }
    }

}
