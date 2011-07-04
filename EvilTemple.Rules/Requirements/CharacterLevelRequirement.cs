#region

using System;

#endregion

namespace EvilTemple.Rules.Requirements
{
    public class CharacterLevelRequirement : IRequirement
    {
        public int MinLevel { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            return critter.EffectiveCharacterLevel >= MinLevel;
        }

        public string ShortDescription
        {
            get { return "Level " + MinLevel; }
        }
    }
}