using System;
using System.Collections.Generic;
using Rules;

namespace EvilTemple.Rules.Requirements
{
    public class AlignmentRequirement : IRequirement
    {
        
        public HashSet<Alignment> Exclusive { get; private set; }

        public HashSet<Alignment> Inclusive { get; private set; }

        public AlignmentRequirement()
        {
            Exclusive = new HashSet<Alignment>();
            Inclusive = new HashSet<Alignment>();
        }

        public bool Satisfied(Critter critter, object context = null)
        {
            if (Inclusive.Count > 0)
                return Inclusive.Contains(critter.Alignment);

            if (Exclusive.Count > 0)
                return !Exclusive.Contains(critter.Alignment);

            return true;
        }

        public string ShortDescription
        {
            get
            {
                if (Inclusive.Count > 0)
                    return string.Join(" ", Inclusive);

                if (Exclusive.Count > 0)
                    return "not " + string.Join(" ", Exclusive);

                return "";
            }
        }
    }
}
