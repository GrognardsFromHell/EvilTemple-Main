#region

using System;
using EvilTemple.Rules.Prototypes;
using Newtonsoft.Json;

#endregion

namespace EvilTemple.Rules
{
    public class Item : BaseObject
    {
        public static readonly Item Default = new Item();

        private Critter _equippedBy;

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is ItemPrototype))
                    throw new ArgumentException("Can only set a ItemPrototype on Item.");
                base.Prototype = value;
            }
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return ItemPrototype.Default; }
        }

        // TODO: Automatically adding/removing from equipment collection when this property is set
        [JsonIgnore]
        public Critter EquippedBy
        {
            get { return _equippedBy; }
            set { _equippedBy = value; }
        }

        public ItemPrototype ItemPrototype
        {
            get { return (ItemPrototype) Prototype; }
        }
    }
}