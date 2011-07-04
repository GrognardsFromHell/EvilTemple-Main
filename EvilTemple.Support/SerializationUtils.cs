using System;
using System.Collections.Generic;
using Rules;

namespace EvilTemple.Support
{
    public static class SerializationUtils
    {
        public static IDictionary<string, object> ToPropertyMap(this object obj)
        {
            var result = new Dictionary<string, object>();

            // Retrieve public properties and serialize them
            foreach (var propInfo in obj.GetType().GetProperties())
            {
                if (!propInfo.CanRead || propInfo.GetIndexParameters().Length > 0)
                    continue;

                var value = propInfo.GetValue(obj, null);

                if (value is Gender)
                {
                    value = ((Gender) value).ToLegacyString();
                }
                else if (value is Alignment)
                {
                    value = ((Alignment)value).ToLegacyString();
                }
                
                if (value is ValueType || value is string)
                {
                    result.Add(CamelizeName(propInfo.Name), value);
                }
            }

            return result;
        }

        private static string CamelizeName(string name)
        {
            if (name.Length > 1)
            {
                return Char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            return name.Length == 1 ? Char.ToLowerInvariant(name[0]).ToString() : name;
        }
    }
}