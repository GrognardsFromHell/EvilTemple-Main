#region



#endregion

using System.Collections.Generic;

namespace EvilTemple.Rules
{
    public class Domain : IIdentifiable
    {
        public static readonly Domain Default = new Domain();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> ClassSkills { get; set; }

        public Domain()
        {
            ClassSkills = new List<string>();
        }
    }
}