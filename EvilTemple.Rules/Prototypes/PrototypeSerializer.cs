#region

using System;
using System.IO;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Rules;

#endregion

namespace EvilTemple.Rules.Prototypes
{
    public sealed class PrototypeSerializer
    {
        /// <summary>
        ///   This map contains mappings between the kernel prototypes and short type names to shorten
        ///   the serialized type reference.
        /// </summary>
        private static readonly TwoWayMap<Type, string> PrototypeMap;

        public static readonly JsonSerializer Serializer;

        static PrototypeSerializer()
        {
            PrototypeMap = new TwoWayMap<Type, string>
                               {
                                   {typeof (ItemPrototype), "Item"},
                                   {typeof (ContainerPrototype), "Container"},
                                   {typeof (CritterPrototype), "Critter"},
                                   {typeof (MapChangerPrototype), "MapChanger"},
                                   {typeof (NonPlayerCharacterPrototype), "NonPlayerCharacter"},
                                   {typeof (PlayerCharacterPrototype), "PlayerCharacter"},
                                   {typeof (PortalPrototype), "Portal"},
                                   {typeof (SceneryPrototype), "Scenery"},
                               };

            Serializer = new JsonSerializer
                             {
                                 TypeNameHandling = TypeNameHandling.Auto,
                                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
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
            Serializer.Converters.Add(new IdentifiableConverter<CharacterClass, CharacterClasses>());
            Serializer.Converters.Add(new IdentifiableConverter<Skill, Skills>());
            Serializer.Converters.Add(new IdentifiableConverter<Deity, Deities>());
            Serializer.Converters.Add(new Vector2Converter());
            Serializer.Converters.Add(new Vector3Converter());
            Serializer.Converters.Add(new Color3Converter());
            Serializer.Converters.Add(new Color4Converter());
            Serializer.Converters.Add(new FeatInstanceConverter());
        }

        public static void Serialize(JsonWriter writer, BaseObjectPrototype value)
        {
            Serializer.Serialize(writer, value, true);
        }

        public static void Serialize(TextWriter writer, BaseObjectPrototype value)
        {
            Serialize(new JsonTextWriter(writer), value);
        }

        public static BaseObjectPrototype Deserialize(JsonReader reader)
        {
            return Serializer.Deserialize<BaseObjectPrototype>(reader);
        }
    }
}