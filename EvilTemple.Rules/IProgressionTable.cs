using System;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace EvilTemple.Rules
{

    public interface IProgressionTable : IIdentifiable
    {
        int this[int level] { get; }
    }

    public class ProgressionTables : Registry<IProgressionTable>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.TypeMapping = new TwoWayMap<Type, string>
                                         {
                                             {typeof(ProgressionTable), "ProgressionTable"}
                                         };
            return serializer;
        }
    }

    public class ProgressionTable : IProgressionTable
    {
        public string Id { get; set; }

        public int[] Table { get; set; }

        public int this[int level]
        {
            get { return Table[level]; }
        }
    }

}