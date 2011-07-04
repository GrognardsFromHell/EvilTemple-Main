#region

using System;
using EvilTemple.Rules;
using EvilTemple.Rules.Prototypes;

#endregion

namespace Rules
{
    public class Container : BaseObject
    {
        public new static readonly Container Default = new Container();

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is ContainerPrototype))
                    throw new ArgumentException("Can only set a ContainerPrototype on Portal.");
                base.Prototype = value;
            }
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return ContainerPrototype.Default; }
        }

        public ContainerPrototype ContainerPrototype
        {
            get { return (ContainerPrototype) Prototype; }
        }
    }
}