using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using EvilTemple.Rules;
using EvilTemple.Rules.Messages;
using EvilTemple.Support;
using Gui;
using Rules;
using EvilTemple.Runtime;
using System.Linq;

namespace EvilTemple.Gui
{
    /// <summary>
    /// Manages the party creation dialog.
    /// </summary>
    public class PartyCreation : Menu
    {
        private readonly IUserInterface _userInterface;
        
        private readonly CharacterVault _vault;

        private readonly CreateCharacter _createCharacter;
        
        public event Action OnCancel;
        
        public Alignment PartyAlignment { get; private set; }

        public PartyCreation(IUserInterface userInterface, CharacterVault vault, CreateCharacter createCharacter)
        {
            _vault = vault;
            _userInterface = userInterface;
            _createCharacter = createCharacter;
            createCharacter.OnCancel += Show;
        }

        public void Show()
        {
            ShowSelectAlignment();
        }

        void ShowSelectAlignment()
        {
            CurrentMenu = _userInterface.AddWidget("interface/ChoosePartyAlignment.qml");

            // Bind events to the menu
            CurrentMenu.cancelled += (Action)Cancel;
            CurrentMenu.alignmentSelected += (Action<string>)(x =>
            {
                PartyAlignment = AlignmentHelper.ConvertFromGui(x);
                ShowPartyCreation();
            });
        }

        void ShowPartyCreation()
        {
            var dialog = _userInterface.AddWidget("interface/PartyVault.qml");

            dialog.characters = GetCharacters();
            dialog.closeClicked += (Action) Cancel;
            dialog.createCharacterClicked += (Action) RequestCharacterCreation;
            dialog.beginAdventuringClicked += (Action)(() => {
                // Get the selected party, create corresponding characters / add them to the party
                // then start the game
                dynamic selectedIds = dialog.getPartySelection();
                CurrentMenu = null;

                var chars = _vault.Characters.Select(t => t.Item2);

                var party = new Party {Alignment = PartyAlignment};

                foreach (var id in selectedIds)
                {
                    var localId = id;
                    var player = chars.First(pc => pc.Id == localId);
                    party.Add(player);
                }

                var campaign = new Campaign {Party = party};
                campaign.Start();
            });

            CurrentMenu = dialog;
        }

        private void RequestCharacterCreation()
        {
            CurrentMenu = null;

            _createCharacter.OnSuccess += ShowPartyCreation;
            _createCharacter.OnCancel += ShowPartyCreation;
            _createCharacter.Show(PartyAlignment);
        }
        
        private void Cancel()
        {
            CurrentMenu = null;
            InvokeOnCancel();
        }

        private void InvokeOnCancel()
        {
            var handler = OnCancel;
            if (handler != null) handler();
        }

        private IEnumerable<object> GetCharacters()
        {
            return _vault.Characters.Select(t => ConvertCharacter(t.Item1, t.Item2));
        }

        private ExpandoObject ConvertCharacter(string filename, PlayerCharacter character)
        {
            dynamic result = new ExpandoObject();
            result.id = character.Id;
            result.name = character.IndividualName;
            result.portrait = character.Portrait.MediumImage;
            result.gender = character.Gender.Translate();
            result.race = character.Race.Name.Translate();
            result.alignment = character.Alignment.Translate();
            result.classes = string.Join(", ", from cl in character.ClassLevels
                                               select cl.CharacterClass.Name.Translate() + " " + cl.Levels); 
            result.filename = filename;
            result.compatible = true; // TODO
            return result;
        }

    }
    
    static class AlignmentHelper
    {
        private static readonly string[] Alignments = { "lg", "ng", "cg", "ln", "n", "cn", "le", "ne", "ce" };

        public static Alignment ConvertFromGui(string selectedAlignment)
        {
            for (var i = 0; i < Alignments.Length; ++i)
                if (Alignments[i] == selectedAlignment)
                    return (Alignment)i;

            throw new InvalidOperationException("Unknown alignment received from GUI: " + selectedAlignment);
        }
    }

}
