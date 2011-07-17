#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using EvilTemple.Runtime;
using Newtonsoft.Json;

#endregion

namespace EvilTemple.Rules.Utilities
{
    public class IdentifiableConverter<TObj, TRegistry> : JsonConverter where TObj : class, IIdentifiable where TRegistry : Registry<TObj>
    {
        private Registry<TObj> _registry;
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(((IIdentifiable) value).Id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            Trace.Assert(reader.ValueType == typeof (string));

            var id = reader.Value as string;

            if (_registry == null)
                _registry = Services.Get<TRegistry>();

            try {
                return _registry[id];
            } catch (KeyNotFoundException e) {
                throw new ArgumentException("Couldn't find identifiable object " + objectType + " with id '" + id + "'", e);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (TObj).IsAssignableFrom(objectType);
        }
    }
}