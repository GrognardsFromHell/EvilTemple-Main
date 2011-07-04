using System;
using System.Collections.Generic;
using System.IO;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using OpenTK;

namespace EvilTemple.Rules.Utilities
{

    public class SkillRankConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ranks = (IDictionary<Skill, int>) value;

            writer.WriteStartObject();

            foreach (var skill in ranks)
            {
                writer.WritePropertyName(skill.Key.Id);
                writer.WriteValue(skill.Value);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ranks = serializer.Deserialize<IDictionary<string, int>>(reader);

            var result = existingValue as IDictionary<Skill, int>;
            if (result == null)
                result = new Dictionary<Skill, int>();
            else
                result.Clear();

            var skills = Services.Get<Skills>();

            foreach (var rank in ranks)
                result[skills[rank.Key]] = rank.Value;
            
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IDictionary<Skill, int>).IsAssignableFrom(objectType);
        }
    }

    public class Vector2Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector2)value;

            writer.WriteStartArray();
            writer.WriteValue(vector.X);
            writer.WriteValue(vector.Y);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var elements = serializer.Deserialize<float[]>(reader);
            if (elements.Length != 2)
                throw new InvalidDataException(string.Format("Read float array for vector2, that has {0} elements.", elements.Length));
            return new Vector2(elements[0], elements[1]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }
    }

    public class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (Vector3)value;

            writer.WriteStartArray();
            writer.WriteValue(vector.X);
            writer.WriteValue(vector.Y);
            writer.WriteValue(vector.Z);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var elements = serializer.Deserialize<float[]>(reader);
            if (elements.Length != 3)
                throw new InvalidDataException(string.Format("Read float array for vector2, that has {0} elements.", elements.Length));
            return new Vector3(elements[0], elements[1], elements[2]);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
}
