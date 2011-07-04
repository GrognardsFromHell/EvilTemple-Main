using System;
using EvilTemple.Runtime;
using Gui;

namespace EvilTemple.Gui
{
    public class MainMenu : Menu
    {

        private readonly IUserInterface _userInterface;

        private readonly PartyCreation _partyCreation;

        public event Action<MainMenu> OnExitGame;
        
        private void InvokeOnExitGame()
        {
            var handler = OnExitGame;
            if (handler != null) handler(this);
        }

        public MainMenu(IUserInterface userInteface, PartyCreation partyCreation)
        {
            _partyCreation = partyCreation;
            _partyCreation.OnCancel += ShowMainMenu;
            _userInterface = userInteface;
        }

        public void ShowMainMenu()
        {
            var dialog = _userInterface.AddWidget("interface/MainMenu.qml");

            // Bind to events on the menu
            dialog.newGameClicked += (Action) StartPartyCreation;
            dialog.exitGameClicked += (Action)InvokeOnExitGame;

            CurrentMenu = dialog;
        }

        void StartPartyCreation()
        {
            CurrentMenu = null;

            _partyCreation.Show();
        }

    }

}
