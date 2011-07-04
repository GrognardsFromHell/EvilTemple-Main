#region

using System;
using System.IO;
using EvilTemple.Rules.Prototypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using NUnit.Framework;

#endregion

namespace Rules.Tests
{
    internal class PrototypeSerializationTest
    {
        private readonly TwoWayMap<Type, string> TypeMapping = new TwoWayMap<Type, string>
                                                                   {
                                                                       {typeof (ItemPrototype), "Item"}
                                                                   };

        [Test]
        public void TestItemPrototype()
        {
            var prototype = new ItemPrototype {Id = "1022"};

            var serializer = new JsonSerializer
                                 {
                                     TypeNameHandling = TypeNameHandling.Objects,
                                     ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                     DefaultValueHandling = DefaultValueHandling.Ignore,
                                     TypeMapping = TypeMapping
                                 };

            var sw = new StringWriter();
            serializer.Serialize(sw, prototype);

            Console.WriteLine(sw.ToString());

            var obj = serializer.Deserialize<BaseObjectPrototype>(new JsonTextReader(new StringReader(sw.ToString())));
            Console.WriteLine(obj);
        }
    }
}