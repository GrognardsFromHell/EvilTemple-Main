using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using EvilTemple.Runtime;
using Newtonsoft.Json;

namespace EvilTemple.Support
{
    public class UserSettings : IUserSettings
    {

        private readonly IPaths _paths;

        // A mapping from key to raw property in JSON form.
        private readonly IDictionary<string, string> _rawProperties;

        // A mapping from key to parsed property. This may be partial and is built on the fly
        private readonly IDictionary<string, object> _parsedProperties;

        // The keys that have changed since the last Flush or Load.
        private readonly ISet<string> _changedKeys;
        
        public UserSettings(IPaths paths)
        {
            _paths = paths;

            _rawProperties = new Dictionary<string, string>();
            _parsedProperties = new Dictionary<string, object>();
            _changedKeys = new HashSet<string>();

            LoadSettings();
        }

        /// <summary>
        /// Loads existing settings from the user data path.
        /// </summary>
        private void LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                Trace.TraceInformation("No configuration file found at {0}", SettingsPath);
                return;
            }

            Trace.TraceInformation("Loading settings from {0}", SettingsPath);

            /*
             * The basic entity is a JSON object. We load all keys as raw strings, then deserialize
             * them on-demand.
             */
            string settingsContent;
            using (var reader = new StreamReader(SettingsPath, Encoding.UTF8))
                settingsContent = reader.ReadToEnd();

            var jsonReader = new JsonTextReader(new StringReader(settingsContent));

            // Skip leading in comment
            while (jsonReader.Read() && jsonReader.TokenType == JsonToken.Comment)
                continue;
                
            while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
            {
                if (jsonReader.TokenType == JsonToken.Comment)
                    continue; // Skip comments (They will be lost)
                    
                Trace.Assert(jsonReader.TokenType == JsonToken.PropertyName, 
                    "Syntax error in configuration file.");

                var key = jsonReader.Value.ToString();

                Trace.Assert(jsonReader.Read(), 
                    "Syntax error in configuration file.");

                var value = new StringBuilder();
                SkipValue(jsonReader, value);

                _rawProperties[key] = value.ToString();
            }
        }

        private static void SkipValue(JsonReader jsonReader, StringBuilder value)
        {
            switch (jsonReader.TokenType)
            {
                case JsonToken.StartObject:
                    value.Append('{');
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
                        SkipValue(jsonReader, value);
                    value.Append('}');
                    break;
                case JsonToken.StartArray:
                    value.Append('[');
                    var firstElement = true;
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                    {
                        if (!firstElement)
                            value.Append(',');
                        firstElement = false;
                        SkipValue(jsonReader, value);
                    }
                    value.Append(']');
                    break;
                case JsonToken.EndObject:
                    value.Append('}');
                    break;
                case JsonToken.EndArray:
                    value.Append(']');
                    break;
                case JsonToken.PropertyName:
                    value.Append(JsonConvert.ToString(jsonReader.Value));
                    value.Append(':');
                    break;
                case JsonToken.Date:
                case JsonToken.Bytes:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Null:
                case JsonToken.Undefined:
                    value.Append(JsonConvert.ToString(jsonReader.Value));
                    break;
                case JsonToken.Comment:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("jsonReader", jsonReader.TokenType, "Unknown JSON Token Type");
            }
        }

        public void Set<T>(string key, T value)
        {
            _parsedProperties[key] = value;
            _changedKeys.Add(key);
        }

        public T Get<T>(string key)
        {
            object parsedValue;
            if (_parsedProperties.TryGetValue(key, out parsedValue))
            {
                if (!typeof(T).IsAssignableFrom(parsedValue.GetType()))
                {
                    throw new InvalidOperationException("The key '" + key + "' has previously been parsed as type '" +
                                                        parsedValue.GetType() + "' but is now being requested as type '" +
                                                        typeof (T) + "'");
                }

                return (T) parsedValue;
            }

            string rawValue;
            if (!_rawProperties.TryGetValue(key, out rawValue))
                throw new ArgumentException("No setting with the key '" + key + "' does exist.");

            var value = JsonConvert.DeserializeObject<T>(rawValue);
            _parsedProperties[key] = value;
            return value;
        }

        public T GetOrSet<T>(string key, T defaultValue)
        {
            if (!Exists(key))
                Set(key, defaultValue);

            return Get<T>(key);
        }

        public bool Exists(string key)
        {
            return _rawProperties.ContainsKey(key)
                || _parsedProperties.ContainsKey(key);
        }

        public void Delete(string key)
        {
            _parsedProperties.Remove(key);
            _rawProperties.Remove(key);
        }

        public ISet<string> Keys
        {
            get { 
                var result = new HashSet<string>();
                foreach (var key in _parsedProperties.Keys)
                    result.Add(key);
                foreach (var key in _rawProperties.Keys)
                    result.Add(key);
                return result;
            }
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        private string SettingsPath
        {
            get { return Path.Combine(_paths.UserDataPath, "settings.json"); }
        }
    }
}
