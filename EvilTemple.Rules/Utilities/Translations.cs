using System.Collections.Generic;
using System.IO;
using System.Text;
using EvilTemple.Runtime;
using Newtonsoft.Json;

namespace EvilTemple.Rules.Utilities
{
    public class Translations : ITranslations
    {
        private readonly IDictionary<string, string> _entries;

        public Translations()
        {
            _entries = new Dictionary<string, string>();
        }

        public string this[string key]
        {
            get
            {
                string result;
                if (!_entries.TryGetValue(key, out result))
                    return "[" + key + "]";
                return result;
            }
        }

        public bool Exists(string key)
        {
            return _entries.ContainsKey(key);
        }

        public void Load(Stream stream)
        {
            var serializer = new JsonSerializer();
            var reader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8));
            var entries = serializer.Deserialize<Dictionary<string,string>>(reader);
            foreach (var entry in entries)
                _entries.Add(entry);
        }

    }
}
