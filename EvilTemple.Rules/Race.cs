#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using EvilTemple.Rules;
using Newtonsoft.Json;

#endregion

namespace Rules
{
    public class Range<T> where T : IConvertible
    {
        public T Min { get; set; }

        public T Max { get; set; }

        internal Range()
        {
        }

        public Range(T min, T max)
        {
            var result = Comparer<T>.Default.Compare(min, max);
            if (result > 0)
            {
                var tmp = max;
                max = min;
                min = tmp;
            }

            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("Min: {0}, Max: {1}", Min, Max);
        }

        public int Interpolate(float factor)
        {
            var culture = CultureInfo.InvariantCulture;
            var min = Min.ToInt32(culture);
            var max = Max.ToInt32(culture);
            return (int) Math.Floor(min + (max - min)*factor);
        }
    }

    public class RangeConverter<T> : JsonConverter where T : IConvertible
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = (Range<T>) value;
            writer.WriteStartArray();
            serializer.Serialize(writer, obj.Min);
            serializer.Serialize(writer, obj.Max);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            Trace.Assert(reader.TokenType == JsonToken.StartArray);

            reader.Read();
            var val1 = serializer.Deserialize<T>(reader);

            reader.Read();
            var val2 = serializer.Deserialize<T>(reader);

            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.EndArray);

            return new Range<T>(val1, val2);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Range<T>);
        }
    }

    public class VisualCharacteristics
    {
        public string Prototype { get; set; }

        [JsonConverter(typeof (RangeConverter<int>))]
        public Range<int> Height { get; set; }

        [JsonConverter(typeof (RangeConverter<int>))]
        public Range<int> Weight { get; set; }
    }

    public class Race : IIdentifiable
    {
        public string Id { get; set; }

        public bool Playable { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public VisualCharacteristics MaleCharacteristics { get; set; }

        public VisualCharacteristics FemaleCharacteristics { get; set; }

        public uint LandSpeed { get; set; }

        public int StartingFeats { get; set; }

        public string EquipmentSubStyleId { get; set; }

        public VisualCharacteristics GetCharacteristics(Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return FemaleCharacteristics;
                case Gender.Other:
                case Gender.Male:
                    return MaleCharacteristics;
                default:
                    throw new ArgumentOutOfRangeException("gender");
            }
        }

        public Race()
        {
            StartingFeats = 1;
        }
    }

    public class RaceNameComparer : IComparer<Race>
    {
        public static readonly RaceNameComparer Instance = new RaceNameComparer();

        public int Compare(Race x, Race y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}