using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;
using Newtonsoft.Json;
using Rules;
using EvilTemple.Runtime;

namespace EvilTemple.Support
{
    public class CharacterVault
    {

        private readonly IPaths _paths;

        private readonly string _userPath;

        /// <summary>
        /// Constructs a character vault object.
        /// </summary>
        /// <param name="paths">Provides path information for several of the game's data files.</param>
        public CharacterVault(IPaths paths)
        {
            _paths = paths;
            _userPath = Path.Combine(_paths.UserDataPath, "Characters");
            
            if (!Directory.Exists(_userPath))
            {
                Directory.CreateDirectory(_userPath);
                Trace.TraceInformation("Creatd user character path: {0}", _userPath);
            }
        }

        /// <summary>
        /// Gets all characters stored in the character vault.
        /// </summary>
        public IList<Tuple<string,PlayerCharacter>> Characters { get { return ReadCharacters(); } }

        /// <summary>
        /// Adds a new character to the vault and returns the corresponding filename.
        /// </summary>
        /// <param name="character">The character to be added.</param>
        /// <returns>The filename generated in the user directory.</returns>
        public string Add(PlayerCharacter character)
        {
            var characterName = character.IndividualName;

            var basename = MangleCharacterName(characterName);

            Trace.TraceInformation("Mangled charname {0} to {1}.", characterName, basename);

            var path = Path.Combine(_userPath, basename + ".json");

            if (File.Exists(path))
            {
                var success = false;
                for (var i = 0; i < 100; ++i)
                {
                    path = string.Format("{0} ({1}).json", basename, i);
                    path = Path.Combine(_userPath, path);

                    if (File.Exists(Path.Combine(_userPath, path)))
                        continue;

                    success = true;
                    break;
                }

                if (!success)
                    throw new InvalidOperationException("Unable to find free filename for: " + basename);
            }

            using (var writer = new JsonTextWriter(new StreamWriter(path, false, Encoding.UTF8)))
            {
                writer.Formatting = Formatting.Indented;
                BaseObjectSerializer.Serializer.Serialize(writer, character, true);
            }

            return Path.GetFileName(path);
        }

        /// <summary>
        ///  Removes a character from the vault given its filename. Only user generated characters may be removed.
        /// </summary>
        /// <param name="filename">Filename relative to the user generated directory.</param>
        public void Remove(string filename)
        {
            
        }

        private const string ReservedCharacters = "<>:\"/\\|?*";

        private static string MangleCharacterName(string name)
        {
             for (var i = 0; i < name.Length; ++i)
             {
                 if (ReservedCharacters.IndexOf(name[i]) != -1)
                     name.Remove(i--);
             }

            return name;
        }

        private IList<Tuple<string,PlayerCharacter>> ReadCharacters()
        {
            Console.WriteLine("Reading Characters: " + _userPath);

            var characters = new List<Tuple<string, PlayerCharacter>>();

            foreach (var file in Directory.EnumerateFiles(_userPath, "*.json"))
            {
                string content;
                try
                {
                    using (var stream = new StreamReader(file, Encoding.UTF8))
                        content = stream.ReadToEnd();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Unable to read character {0}: {1}", file, e);
                    continue;
                }

                try
                {
                    var ch = BaseObjectSerializer.Deserialize(new JsonTextReader(new StringReader(content)));
                    var playerCh = ch as PlayerCharacter;
                    if (playerCh == null)
                    {
                        Trace.TraceError("File {0} doesn't contain a player character.", file);
                        continue;
                    }

                    Trace.TraceInformation("Read character {0} ({1}) from {2}", ch.IndividualName, ch.Id, file);
                    characters.Add(Tuple.Create(file, playerCh));
                }
                catch (Exception e)
                {
                    Trace.TraceError("Unable to parse character {0}: {1}", file, e);
                }
            }

            return characters;
        }
    }
    
}
