#region

using System;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;

#endregion

namespace Rules
{
    public class NonPlayerCharacter : Critter
    {
        public static readonly NonPlayerCharacter Default = new NonPlayerCharacter();

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (typeof (NonPlayerCharacterPrototype) != value.GetType())
                    throw new ArgumentException("Can only set a NonPlayerCharacterPrototype on NonPlayerCharacters.");
                base.Prototype = value;
            }
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return NonPlayerCharacterPrototype.Default; }
        }

        public NonPlayerCharacterPrototype NonPlayerCharacterPrototype
        {
            get { return (NonPlayerCharacterPrototype) Prototype; }
        }
    }
}