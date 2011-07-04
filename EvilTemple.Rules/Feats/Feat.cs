#region

using System;
using System.Collections.Generic;
using System.IO;
using EvilTemple.Rules.Requirements;
using EvilTemple.Runtime;
using Newtonsoft.Json;

#endregion

namespace EvilTemple.Rules.Feats
{
    /// <summary>
    ///   Describes a feat that can be taken by critters to grant additional abilities.
    /// </summary>
    public class Feat : IIdentifiable
    {
        /// <summary>
        ///   The unique identifier of this feat.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///   True if a combination of this feat can only be taken once. This includes
        ///   the parameters.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        ///   The human readable name for this feat.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   Short description for this feat.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        ///   Category id for this feat.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        ///   A parameter that must be chosen for this feat.
        /// </summary>
        public FeatParameter Parameter { get; private set; }

        /// <summary>
        ///   Requirements that have to be met in order to take this feat.
        /// </summary>
        public List<IRequirement> Requirements { get; private set; }

        public Feat()
        {
            Requirements = new List<IRequirement>();
            Unique = true;
        }
    }

    /// <summary>
    ///   Describes a choice that can be made in addition to a feat.
    ///   For instance the type of weapon that the weapon focus feat applies to
    ///   is a feat parameter.
    /// </summary>
    public class FeatParameter
    {
        public string Name;

        public string Description;

        public IDictionary<string, string> Values;
    }

    /// <summary>
    ///   Represents a feat that has been taken by a critter. In addition to containing
    ///   a reference to the actual feat,
    /// </summary>
    public sealed class FeatInstance : IEquatable<FeatInstance>
    {
        public FeatInstance()
        {
        }

        public FeatInstance(string featId, string parameterValue = null)
        {
            FeatId = featId;
            ParameterValue = parameterValue;
        }

        public string FeatId { get; set; }

        public string ParameterValue { get; set; }

        public string ShortDescription {
            get { var feat = Services.Get<FeatRegistry>()[FeatId];
            if (ParameterValue == null)
                return feat.Name;
            return feat.Name + " (" + feat.Parameter.Values[ParameterValue] + ")";
            }
        }

        public bool Equals(FeatInstance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FeatId, FeatId) && Equals(other.ParameterValue, ParameterValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FeatInstance)) return false;
            return Equals((FeatInstance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FeatId != null ? FeatId.GetHashCode() : 0)*397) ^ (ParameterValue != null ? ParameterValue.GetHashCode() : 0);
            }
        }

        public static bool operator ==(FeatInstance left, FeatInstance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FeatInstance left, FeatInstance right)
        {
            return !Equals(left, right);
        }
    }

    public class FeatInstanceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var instance = (FeatInstance) value;

            if (instance.ParameterValue == null)
            {
                writer.WriteValue(instance.FeatId);
            }
            else
            {
                writer.WriteStartArray();
                writer.WriteValue(instance.FeatId);
                writer.WriteValue(instance.ParameterValue);
                writer.WriteEndArray();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new FeatInstance {FeatId = (string)reader.Value};
            }
            if (reader.TokenType == JsonToken.StartArray)
            {
                var elements = serializer.Deserialize<string[]>(reader);
                if (elements.Length != 2)
                    throw new InvalidDataException("Feat instance may only have one parameter.");

                return new FeatInstance {FeatId = elements[0], ParameterValue = elements[1]};
            }
            throw new InvalidDataException("Unable to deserialize the feat instance.");
        }

        public override bool CanConvert(Type objectType)
        {
            // FeatInstance is sealed, so direct comparison is valid
            return typeof (FeatInstance) == objectType;
        }
    }
    
}