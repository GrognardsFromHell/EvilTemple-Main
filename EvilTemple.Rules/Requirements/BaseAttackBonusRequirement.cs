using System;

namespace EvilTemple.Rules.Requirements
{
    public class BaseAttackBonusRequirement : IRequirement
    {
        public int Bonus { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            return critter.BaseAttackBonus >= Bonus;
        }

        public string ShortDescription
        {
            get { return "Base Attack +" + Bonus; }
        }
    }
}