#region

using System;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;

#endregion

namespace Rules
{
    public class MapChanger : BaseObject
    {
        public static readonly MapChanger Default = new MapChanger();

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is MapChangerPrototype))
                    throw new ArgumentException("Can only set a MapChangerPrototype on MapChanger.");
                base.Prototype = value;
            }
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return MapChangerPrototype.Default; }
        }

        public MapChangerPrototype MapChangerPrototype
        {
            get { return (MapChangerPrototype) Prototype; }
        }

        public JumpPoint TeleportTarget { get; set; }
    }
}