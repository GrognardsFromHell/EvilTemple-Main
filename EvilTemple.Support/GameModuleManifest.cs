using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EvilTemple.Rules;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rules;

namespace EvilTemple.Support
{

    /// <summary>
    /// A module is a set of game data.
    /// </summary>
    public class GameModuleManifest : IIdentifiable
    {

        public static readonly GameModuleManifest Default = new GameModuleManifest();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ISet<string> DependsOn { get; set; }

        public string Author { get; set; }

        public string Version { get; set; }

        public List<string> ParticleSystems { get; set; }

        public List<string> Prototypes { get; set; }

        public List<string> Maps { get; set; }

        public List<string> Equipment { get; set; }

        public List<string> HairStyles { get; set; }

        public List<string> InventoryIcons { get; set; }

        public List<string> JumpPoints { get; set; }

        public List<string> Portraits { get; set; }

        public List<string> Translations { get; set; }

        public List<string> Voices { get; set; }

        public GameModuleManifest()
        {
            ParticleSystems = new List<string>();
            Prototypes = new List<string>();
            Maps = new List<string>();
            Equipment = new List<string>();
            HairStyles = new List<string>();
            InventoryIcons = new List<string>();
            JumpPoints = new List<string>();
            Portraits = new List<string>();
            Translations = new List<string>();
            Voices = new List<string>();
        }

        public byte[] Save()
        {
            var outputStream = new MemoryStream();
            var output = new StreamWriter(outputStream, Encoding.UTF8);
            var writer = new JsonTextWriter(output);
            writer.Formatting = Formatting.Indented;
            var serializer = CreateSerializer();
            serializer.Serialize(writer, this);
            writer.Close();
            return outputStream.ToArray();
        }

        public static GameModuleManifest Read(string path)
        {
            using (var zipFileStream = new ZipInputStream(new FileStream(path, FileMode.Open)))
            {
                ZipEntry entry;
                while ((entry = zipFileStream.GetNextEntry()) != null)
                {
                    if (!entry.IsFile || entry.Name.ToLowerInvariant() != "module.json")
                        continue;

                    if (!entry.CanDecompress)
                        throw new InvalidDataException("Unsupported compression method used for " + path);

                    var serializer = CreateSerializer();
                    var streamReader = new StreamReader(zipFileStream, Encoding.UTF8);
                    return serializer.Deserialize<GameModuleManifest>(new JsonTextReader(streamReader));
                }
            }

            return null;
        }

        public static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return serializer;
        }
    }

}
