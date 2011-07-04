using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using Rules;

namespace EvilTemple.Rules
{
    public class Deities : Registry<Deity>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();

            serializer.Converters.Add(new IdentifiableConverter<Domain, Domains>());

            return serializer;
        }
    }

    public class Deity : IIdentifiable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Alignment Alignment { get; set; }

        public List<Domain> Domains { get; set; }
    }
}
