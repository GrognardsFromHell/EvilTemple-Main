using System;

namespace EvilTemple.Rules.Requirements
{
    public class ConditionalRequirement : IRequirement
    {
        public string Condition { get; set; }

        public IRequirement Requirement { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            get { return Condition + " " + Requirement.ShortDescription; }
        }
    }
}