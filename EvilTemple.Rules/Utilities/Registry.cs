#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#endregion

namespace EvilTemple.Rules.Utilities
{
    public abstract class Registry<T> where T : class, IIdentifiable
    {
        public readonly Dictionary<string, T> Objects = new Dictionary<string, T>();

        public T this[string id]
        {
            get
            {
#if TRACE
                if (!Objects.ContainsKey(id))
                {
                    var t = typeof (T);
                    Trace.TraceError("Unable to find {0} with id '{1}'", t.Name, id);
                }
#endif
                return Objects[id];
            }
        }

        /// <summary>
        ///   Checks whether a given object exists.
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public bool Contains(string id)
        {
            return Objects.ContainsKey(id);
        }

        /// <summary>
        ///   Adds a new object to this registry.
        /// </summary>
        /// <param name = "value">The new object to add. Must not be null.</param>
        public void Add(T value)
        {
            Objects.Add(value.Id, value);
        }

        /// <summary>
        ///   Load additional races from the given input stream.
        /// </summary>
        /// <param name = "reader">A stream that contains race objects in serialized JSON form.</param>
        public void Load(JsonTextReader reader)
        {
            var jsonSerializer = CreateSerializer();

            if (!reader.Read())
                throw new ArgumentException("The given JSON reader is empty.");

            SkipComments(reader);

            if (reader.TokenType != JsonToken.StartArray)
                throw new ArgumentException("The given JSON reader is not positioned at an array.");

            while (reader.Read())
            {
                SkipComments(reader);

                if (reader.TokenType == JsonToken.StartObject)
                {
                    var obj = jsonSerializer.Deserialize<T>(reader);
                    if (obj == null)
                        throw new InvalidDataException("Unable to deserialize object @ " 
                            + reader.LineNumber + "," + reader.LinePosition);
                    Add(obj);
                }
                else if (reader.TokenType == JsonToken.EndArray)
                    break;
                else
                    throw new InvalidDataException("Invalid JSON Token Type: " + reader.TokenType);
            }
        }

        private static void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();
        }

        public void Save(JsonWriter writer)
        {
            var jsonSerializer = CreateSerializer();

            writer.WriteStartArray();

            foreach (var o in Objects)
                jsonSerializer.Serialize(writer, o.Value);

            writer.WriteEndArray();
        }

        /// <summary>
        ///   Create the JSON serializer that will be used to de-/serialize the objects in this registry.
        /// </summary>
        /// <returns>A preconfigured JsonSerializer.</returns>
        protected virtual JsonSerializer CreateSerializer()
        {
            var result = new JsonSerializer
                       {
                           TypeNameHandling = TypeNameHandling.Auto,
                           ContractResolver = new CamelCasePropertyNamesContractResolver(),
                           DefaultValueHandling = DefaultValueHandling.Ignore
                       };
            result.Converters.Add(new StringEnumConverter());
            return result;
        }
    }
}