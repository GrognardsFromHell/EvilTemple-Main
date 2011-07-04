#region

using System.Collections.Generic;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using Rules;

#endregion

namespace EvilTemple.Rules.Prototypes
{
    public class Prototypes : Registry<BaseObjectPrototype>
    {
        public Prototypes()
        {
            Add(new StaticGeometryPrototype());
        }

        protected override JsonSerializer CreateSerializer()
        {
            return PrototypeSerializer.Serializer;
        }
    }

    /************************************************************************/
    /* TODO                                                                 */
    /************************************************************************/

    public class ItemPrototype : BaseObjectPrototype
    {
        public new static readonly ItemPrototype Default = new ItemPrototype();

        public InventoryIcon InventoryIcon { get; set; }

        /// <summary>
        ///   Defines the slots on which this item may be equipped.
        /// </summary>
        public ISet<EquipmentSlot> EquipmentSlots { get; set; }

        /// <summary>
        ///   Defines how this item looks when equipped by the character. May be null 
        ///   in case the item has no visual effect on the character.
        /// </summary>
        public EquipmentStyle EquipmentStyle { get; set; }

        public override BaseObject Instantiate()
        {
            return new Item();
        }
    }

    public class ContainerPrototype : BaseObjectPrototype
    {
        public new static readonly ContainerPrototype Default = new ContainerPrototype();

        public override BaseObject Instantiate()
        {
            return new Container();
        }
    }

    public class PortalPrototype : BaseObjectPrototype
    {
        public new static readonly PortalPrototype Default = new PortalPrototype();

        public override BaseObject Instantiate()
        {
            return new Portal();
        }
    }

    public class MapChangerPrototype : BaseObjectPrototype
    {
        public new static readonly MapChangerPrototype Default = new MapChangerPrototype();

        public MapChangerPrototype()
        {
            Interactive = true;
        }

        public override BaseObject Instantiate()
        {
            return new MapChanger();
        }
    }

    public class NonPlayerCharacterPrototype : CritterPrototype
    {
        public new static readonly NonPlayerCharacterPrototype Default = new NonPlayerCharacterPrototype();

        public override BaseObject Instantiate()
        {
            return new NonPlayerCharacter();
        }
    }

    public class PlayerCharacterPrototype : CritterPrototype
    {
        public new static readonly PlayerCharacterPrototype Default = new PlayerCharacterPrototype();

        public override BaseObject Instantiate()
        {
            return new PlayerCharacter();
        }
    }

    public class SceneryPrototype : BaseObjectPrototype
    {
        public new static readonly SceneryPrototype Default = new SceneryPrototype();

        public override BaseObject Instantiate()
        {
            return new Scenery();
        }
    }

    /// <summary>
    /// This prototype is used for static geometry (like a river) that is placed on the map.
    /// It is never serialized, and has the special identifier "StaticGeometry".
    /// </summary>
    public class StaticGeometryPrototype : SceneryPrototype
    {
        public StaticGeometryPrototype()
        {
            Id = "StaticGeometry";
        }
    }
}