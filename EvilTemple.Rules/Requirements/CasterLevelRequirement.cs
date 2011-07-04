using System;

namespace EvilTemple.Rules.Requirements
{
    public class CasterLevelRequirement : IRequirement
    {
        public CasterLevelRequirementType Type { get; set; }

        public int MinLevel { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            get { return "Caster Level " + MinLevel; }
        }
    }

    public enum CasterLevelRequirementType
    {
        Any,
        Arcane,
        Divine
    }
}