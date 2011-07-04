using System;
using EvilTemple.Runtime;
using Microsoft.Scripting.Hosting;

namespace EvilTemple.Gui
{
    class Console
    {
        private dynamic _consoleWindow;

        private IUserInterface _userInterface;

        private IScripting _scripting;

        private ScriptScope _scope;

        public Console(IUserInterface userInterface, IScripting scripting)
        {
            _userInterface = userInterface;
            _scripting = scripting;
            _scope = CreateScope();
        }

        public void ToggleVisibility()
        {
            if (_consoleWindow == null)
            {
                _consoleWindow = _userInterface.AddWidget(@"interface/Console.qml");
                _consoleWindow.closeClicked += (Action) (() =>
                                                     {
                                                         _consoleWindow.deleteLater();
                                                         _consoleWindow = null;
                                                     });
                _consoleWindow.commandIssued += (Action<string>) (HandleCommand);
            }
            else
            {
                _consoleWindow.deleteLater();
                _consoleWindow = null;
            }
        }

        private ScriptScope CreateScope()
        {
            var scope = _scripting.Engine.CreateScope();

            // Transfer a set of bindings to the scope

            return scope;
        }

        private void HandleCommand(string command)
        {
            try
            {
                var result = _scope.Engine.Execute(command, _scope);
                if (result != null)
                    _consoleWindow.appendResult(result.ToString());
            } catch (Exception e) {
                _consoleWindow.appendResult("<b style='color: red'>ERROR:</b> " + e);
            }
        }
    }
}
