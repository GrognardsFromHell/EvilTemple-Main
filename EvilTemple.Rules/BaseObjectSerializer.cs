using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Prototypes;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Rules;

namespace EvilTemple.Rules
{
    public class BaseObjectSerializer
    {
        /// <summary>
        ///   This map contains mappings between the kernel object types and short type names to shorten
        ///   the serialized type reference.
        /// </summary>
        private static readonly TwoWayMap<Type, string> PrototypeMap;

        public static readonly JsonSerializer Serializer;

        static BaseObjectSerializer()
        {
            PrototypeMap = new TwoWayMap<Type, string>
                               {
                                   {typeof (Item), "Item"},
                                   {typeof (Container), "Container"},
                                   {typeof (Critter), "Critter"},
                                   {typeof (MapChanger), "MapChanger"},
                                   {typeof (NonPlayerCharacter), "NonPlayerCharacter"},
                                   {typeof (PlayerCharacter), "PlayerCharacter"},
                                   {typeof (Portal), "Portal"},
                                   {typeof (Scenery), "Scenery"},
                               };

            Serializer = new JsonSerializer
                             {
                                 TypeNameHandling = TypeNameHandling.Auto,
                                 ContractResolver = new CamelCasePropertyNamesContractResolver { ShouldSerializeReadOnlyMembers = false },
                                 DefaultValueHandling = DefaultValueHandling.Ignore,
                                 TypeMapping = PrototypeMap
                             };

            // IIdentifiables referenced by prototypes should be serialized by-ref, not by-value
            Serializer.Converters.Add(new StringEnumConverter());
            Serializer.Converters.Add(new IdentifiableConverter<InventoryIcon, InventoryIcons>());
            Serializer.Converters.Add(new IdentifiableConverter<EquipmentStyle, EquipmentStyles>());
            Serializer.Converters.Add(new IdentifiableConverter<Portrait, Portraits>());
            Serializer.Converters.Add(new IdentifiableConverter<HairStyle, HairStyles>());
            Serializer.Converters.Add(new IdentifiableConverter<Race, Races>());
            Serializer.Converters.Add(new IdentifiableConverter<PlayerVoice, PlayerVoices>());
            Serializer.Converters.Add(new IdentifiableConverter<BaseObjectPrototype, Prototypes.Prototypes>());
            Serializer.Converters.Add(new IdentifiableConverter<CharacterClass, CharacterClasses>());
            Serializer.Converters.Add(new IdentifiableConverter<Skill, Skills>());
            Serializer.Converters.Add(new IdentifiableConverter<Deity, Deities>());
            Serializer.Converters.Add(new IdentifiableConverter<Domain, Domains>());
            Serializer.Converters.Add(new IdentifiableConverter<JumpPoint, JumpPoints>());
            Serializer.Converters.Add(new Vector2Converter());
            Serializer.Converters.Add(new Vector3Converter());
            Serializer.Converters.Add(new Color3Converter());
            Serializer.Converters.Add(new Color4Converter());
            Serializer.Converters.Add(new FeatInstanceConverter());
        }

        public static void Serialize(JsonWriter writer, BaseObject value)
        {
            Serializer.Serialize(writer, value, true);
        }

        public static void Serialize(TextWriter writer, BaseObject value)
        {
            Serialize(new JsonTextWriter(writer), value);
        }

        public static BaseObject Deserialize(JsonReader reader)
        {
            return Serializer.Deserialize<BaseObject>(reader);
        }
    }
}
