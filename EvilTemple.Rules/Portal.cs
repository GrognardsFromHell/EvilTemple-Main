#region

using System;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;

#endregion

namespace Rules
{
    public class Portal : BaseObject
    {
        public static readonly Portal Default = new Portal();

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is PortalPrototype))
                    throw new ArgumentException("Can only set a PortalPrototype on Portal.");
                base.Prototype = value;
            }
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return PortalPrototype.Default; }
        }

        public PortalPrototype PortalPrototype
        {
            get { return (PortalPrototype) Prototype; }
        }
    }
}