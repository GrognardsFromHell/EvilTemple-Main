#region

using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;

#endregion

namespace Rules
{
    public sealed class Races : Registry<Race>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.TypeMapping.Add(typeof (Race), "Race");
            return serializer;
        }
    }
}