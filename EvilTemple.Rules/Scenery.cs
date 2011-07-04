#region

using System;
using EvilTemple.Rules.Prototypes;
using Rules;

#endregion

namespace EvilTemple.Rules
{
    public class Scenery : BaseObject
    {
        public static Scenery Default = new Scenery();

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return SceneryPrototype.Default; }
        }

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is SceneryPrototype))
                    throw new ArgumentException("Can only set a SceneryPrototype on SceneryObject.");
                base.Prototype = value;
            }
        }
    }
}