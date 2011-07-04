using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    /// <summary>
    /// Allows access to a per-user storage for settings.
    /// This storage can also be used by third party modules to store their settings.
    /// 
    /// Settings are stored using the JSON format. Most objects may be stored in the setting
    /// storage.
    /// </summary>
    public interface IUserSettings
    {

        void Set<T>(string key, T value);

        /// <summary>
        /// Gets a certain value from the settings storage. Throws an exception if the key doesn't
        /// exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        T GetOrSet<T>(string key, T defaultValue);

        bool Exists(string key);

        void Delete(string key);

        ISet<string> Keys { get; }

        /// <summary>
        /// Flush changed settings to disc immediately.
        /// </summary>
        void Flush();

    }
}
