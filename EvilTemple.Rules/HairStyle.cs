using System.Collections.Generic;
using System.Linq;
using EvilTemple.Rules.Utilities;
using Rules;

namespace EvilTemple.Rules
{

    public class HairStyles : Registry<HairStyle>
    {

        /// <summary>
        /// Returns a list of hair styles for a specific gender, sorted by their translated name.
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public IList<HairStyle> GetByGender(Gender gender)
        {
            var result = from x in Objects where x.Value.Gender == gender select x.Value;
            return result.ToList();
        }

        public static HairStyle None { get; private set; }

        static HairStyles()
        {
            None = new HairStyle {Id = "none", Name = "(none)"};
        }
    }

    public class HairStyle : IIdentifiable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IDictionary<string, HairSubStyle> SubStyles { get; private set; }

        public Gender Gender { get; private set; }

        public HairStyle()
        {
            SubStyles = new Dictionary<string, HairSubStyle>();
        }

        public HairSubStyle GetSubStyle(Race race)
        {
            var id = race.Id;
            return SubStyles[id];
        }
    }

    public class HairSubStyle
    {
        public string BigModel { get; set; }

        public string SmallModel { get; set; }

        public string NoneModel { get; set; }
    }
}
