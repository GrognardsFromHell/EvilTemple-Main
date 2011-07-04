#region



#endregion

using System;
using EvilTemple.Runtime;

namespace EvilTemple.Rules.Requirements
{
    public class ClassLevelRequirement : IRequirement
    {
        public CharacterClass Class
        {
            get { return Services.Get<CharacterClasses>()[ClassId]; }
        }

        public string ClassId { get; set; }

        public int MinLevel { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            var classLevels = critter.GetClassLevel(Class);
            return classLevels != null && classLevels.Levels >= MinLevel;
        }

        public string ShortDescription
        {
            get { return Class.Name + " " + MinLevel; }
        }
    }
}