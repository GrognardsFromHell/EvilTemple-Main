#region

using System;
using EvilTemple.Rules.Prototypes;

#endregion

namespace EvilTemple.Rules
{
    public class PlayerCharacter : Critter
    {
        public static PlayerCharacter Default = new PlayerCharacter();

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return PlayerCharacterPrototype.Default; }
        }

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is PlayerCharacterPrototype))
                    throw new ArgumentException("Can only set a PlayerCharacterPrototype on PlayerCharacter.");
                base.Prototype = value;
            }
        }

        public PlayerVoice Voice { get; set; }
    }
}