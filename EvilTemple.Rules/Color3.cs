using System;
using System.Diagnostics;
using Newtonsoft.Json;
using OpenTK;

namespace EvilTemple.Rules
{
    public struct Color4 : IEquatable<Color4>
    {
        public byte Alpha { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public float AlphaF
        {
            get { return Alpha/255.0f; }
        }

        public float RedF
        {
            get { return Red / 255.0f; }
        }

        public float GreenF
        {
            get { return Green / 255.0f; }
        }

        public float BlueF
        {
            get { return Blue / 255.0f; }
        }

        public bool Equals(Color4 other)
        {
            return other.Alpha == Alpha && other.Red == Red && other.Green == Green && other.Blue == Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Color4)) return false;
            return Equals((Color4) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Alpha.GetHashCode();
                result = (result*397) ^ Red.GetHashCode();
                result = (result*397) ^ Green.GetHashCode();
                result = (result*397) ^ Blue.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Color4 left, Color4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color4 left, Color4 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("R: {1}, G: {2}, B: {3}, A: {0}", Alpha, Red, Green, Blue);
        }
    }

    public struct Color3 : IEquatable<Color3>
    {
        public Color3(byte red, byte green, byte blue) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red { get; set; }

        [JsonIgnore]
        public float RedF
        {
            get { return Red / 255.0f; }
        }

        public byte Green { get; set; }

        [JsonIgnore]
        public float GreenF
        {
            get { return Green / 255.0f; }
        }

        public byte Blue { get; set; }

        [JsonIgnore]
        public float BlueF
        {
            get { return Blue / 255.0f; }
        }

        public Vector4 ToVector4()
        {
            return new Vector4(RedF, GreenF, BlueF, 1);
        }

        public bool Equals(Color3 other)
        {
            return other.Red == Red && other.Green == Green && other.Blue == Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Color3)) return false;
            return Equals((Color3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Red.GetHashCode();
                result = (result*397) ^ Green.GetHashCode();
                result = (result*397) ^ Blue.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(Color3 left, Color3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color3 left, Color3 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Red, Green, Blue);
        }
    }

    public class Color3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color3) value;

            writer.WriteStartArray();
            writer.WriteValue(color.Red);
            writer.WriteValue(color.Green);
            writer.WriteValue(color.Blue);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Trace.Assert(reader.TokenType == JsonToken.StartArray);
            
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            var result = new Color3();
            result.Red = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            result.Green = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            result.Blue = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.EndArray);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Color3).IsAssignableFrom(objectType);
        }
    }

    public class Color4Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color4)value;

            writer.WriteStartArray();
            writer.WriteValue(color.Red);
            writer.WriteValue(color.Green);
            writer.WriteValue(color.Blue);
            writer.WriteValue(color.Alpha);
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Trace.Assert(reader.TokenType == JsonToken.StartArray);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            var result = new Color4();
            result.Red = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            result.Green = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            result.Blue = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.Integer);
            result.Alpha = Convert.ToByte(reader.Value);
            reader.Read();
            Trace.Assert(reader.TokenType == JsonToken.EndArray);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Color3).IsAssignableFrom(objectType);
        }
    }

}
