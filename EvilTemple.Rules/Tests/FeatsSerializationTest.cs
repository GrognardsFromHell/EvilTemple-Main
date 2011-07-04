#region

using System;
using System.IO;
using EvilTemple.Rules.Feats;
using Newtonsoft.Json;
using NUnit.Framework;

#endregion

namespace Rules.Tests
{
    public class FeatsSerializationTest
    {
        [Test]
        public void TestSerialize()
        {
            var tw = new StringWriter();

            var writer = new JsonTextWriter(tw);
            writer.QuoteChar = '\'';
            writer.Formatting = Formatting.Indented;

            var feats = new FeatRegistry();

            var feat = new Feat {Id = "abc'df"};
            feats.Add(feat);

            feats.Save(writer);

            Console.WriteLine(tw.ToString());

            var reader = new StringReader(tw.ToString());
            feats = new FeatRegistry();
            feats.Load(new JsonTextReader(reader));
        }
    }
}