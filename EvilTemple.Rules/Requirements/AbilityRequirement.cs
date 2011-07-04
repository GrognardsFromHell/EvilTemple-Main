using System;

namespace EvilTemple.Rules.Requirements
{
    public class AbilityRequirement : IRequirement
    {
        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            // TODO: Implement
            get { return "Abi"; }
        }
    }
}