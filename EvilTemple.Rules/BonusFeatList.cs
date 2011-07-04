using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;

namespace EvilTemple.Rules
{
    
    /// <summary>
    /// Describes a list of feats that can be taken as a bonus feat. Multiple lists can exist.
    /// Examples are the list of warrior bonus feats, or the list of wizard bonus feats.
    /// </summary>
    public class BonusFeatList : IIdentifiable
    {
        public string Id { get; set; }

        public List<FeatInstance> Feats { get; set; }

        public BonusFeatList()
        {
            Feats = new List<FeatInstance>();
        }

        public bool Contains(FeatInstance featInstance)
        {
            /*
             * Keep in mind that a list that contains "Weapon Focus (ANY)" will contain "Weapon Focus (Longsword)", but not the
             * other way around.
             */
            return Feats.Any(fi =>
                                 {
                                     if (fi.FeatId != featInstance.FeatId)
                                         return false;

                                     if (fi.ParameterValue == null)
                                         return true;

                                     return fi.ParameterValue == featInstance.ParameterValue;
                                 });
        }
    }
    
    public class BonusFeatLists : Registry<BonusFeatList>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();

            serializer.Converters.Add(new FeatInstanceConverter());

            return serializer;
        }
    }

}
