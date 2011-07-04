using System;
using Gui;
using Ninject.Modules;
using Ninject;
using EvilTemple.Runtime;
using EvilTemple.Runtime.Messages;

namespace EvilTemple.Gui
{
    public class GuiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainMenu>().ToSelf();
            Bind<PartyCreation>().ToSelf();
            Bind<CreateCharacter>().ToSelf();
            Bind<Console>().ToSelf().InSingletonScope();
            EventBus.Register<ApplicationStartup>(ShowMainMenu);

            Kernel.Get<IShortcuts>().Register(Keys.F12, ShowConsole);
        }

        private void ShowConsole()
        {
            Kernel.Get<Console>().ToggleVisibility();
        }

        private void ShowMainMenu(ApplicationStartup message)
        {
            var gameView = Kernel.Get<IGameView>();

            var mainMenu = Kernel.Get<MainMenu>();
            // mainMenu.OnExitGame += delegate { gameView.Close(); };
            mainMenu.ShowMainMenu();
        }
    }
}
