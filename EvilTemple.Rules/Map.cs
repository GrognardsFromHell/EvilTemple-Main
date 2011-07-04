using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTK;

namespace EvilTemple.Rules
{
    public class Map : IIdentifiable
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public Vector3 StartPosition { get; set; }

        public MapInfo MapInfo { get; set; }

        public Map(MapInfo mapInfo)
        {
            MapInfo = mapInfo;
            Id = mapInfo.Id;
            Name = mapInfo.Name;
            StartPosition = mapInfo.StartPosition;
        }
    }

    public class Maps
    {

        public Dictionary<string, Map> Mapping { get; set; }

        public Maps()
        {
            Mapping = new Dictionary<string, Map>();
        }

        public Map this[string id]
        {
            get { return Mapping[id]; }
        }

        public void LoadMap(Stream stream)
        {
            using (var reader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8)))
            {
                var serializer = new JsonSerializer
                                     {
                                         ContractResolver = new CamelCasePropertyNamesContractResolver()
                                     };
                serializer.Converters.Add(new Vector2Converter());
                serializer.Converters.Add(new Vector3Converter());
                var mapInfo = serializer.Deserialize<MapInfo>(reader);
                Console.WriteLine(mapInfo.Id);
                Mapping[mapInfo.Id] = new Map(mapInfo);
            }
        }

    }

    public class MapInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Vector2 ScrollBoxMin { get; set; }

        public Vector2 ScrollBoxMax { get; set; }

        public string Area { get; set; }

        public string DayBackground { get; set; }

        public string NightBackground { get; set; }

        public Vector3 StartPosition { get; set; }

        public bool Outdoors { get; set; }

        public bool Unfogged { get; set; }

        public bool MenuMap { get; set; }

        public bool TutorialMap { get; set; }

        public string RegionsFile { get; set; }

        public List<string> SoundSchemes { get; set; }

        public GlobalLightingSettings Lighting { get; set; }

        public string DynamicLightsFile { get; set; }

        public string ClippingGeometryFile { get; set; }

        public string StaticObjectsFiles { get; set; }

        public string DynamicObjectsFile { get; set; }

        public uint FirstEntryMovie { get; set; }

        public string ParticleSystemsFile { get; set; }
    }
}
