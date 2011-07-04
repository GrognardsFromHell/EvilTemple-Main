#region

using System;

#endregion

namespace EvilTemple.Rules.Prototypes
{
    public class BaseObjectPrototype : IIdentifiable
    {
        public static readonly BaseObjectPrototype Default = new BaseObjectPrototype();

        public string Id { get; set; }

        public uint Scale { get; set; }

        public float Rotation { get; set; }

        public bool Interactive { get; set; }

        public bool DrawBehindWalls { get; set; }

        public uint HitPoints { get; set; }

        public uint TemporaryHitPoints { get; set; }

        public uint DamageTaken { get; set; }

        public uint SubdualDamageTaken { get; set; }

        public string IndividualName { get; set; }

        public Flags Flags { get; set; }

        public bool DontDraw { get; set; }

        public bool Disabled { get; set; }

        public bool Unlit { get; set; }

        public string InternalDescription { get; set; }

        public string InternalId { get; set; }

        public string DescriptionId { get; set; }

        public Size Size { get; set; }

        public ObjectMaterial Material { get; set; }

        public uint SoundId { get; set; }

        public string CategoryId { get; set; }

        public string Model { get; set; }

        public uint SelectionRadius { get; set; }

        public int SelectionHeight { get; set; }

        public BaseObjectPrototype()
        {
            Scale = 100;
            Rotation = 0;
            Interactive = true;
            DrawBehindWalls = false;
            HitPoints = 1;
        }

        /// <summary>
        ///   Instantiates an object that is associated with this prototype.
        /// </summary>
        /// <remarks>
        ///   Subclasses should override this method and create objects of the class that
        ///   corresponds to their type.
        /// </remarks>
        /// <returns>A new object that is associated with this prototype.</returns>
        public virtual BaseObject Instantiate()
        {
            return new BaseObject
                       {
                           Prototype = this
                       };
        }
    }

    [Flags]
    public enum Flags
    {
        ClickThrough,
        DontDraw,
        DontLight,
        Invisible,
        Invulnerable,
        NoBlock,
        NoHeight,
        Off,
        RandomSize,
        SeeThrough,
        ShootThrough,
        Wading,
        WaterWalking,
        ProvidesCover
    }

    public enum ObjectMaterial
    {
        Powder,
        Fire,
        Force,
        Gas,
        Paper,
        Liquid,
        Cloth,
        Glass,
        Metal,
        Flesh,
        Plant,
        Wood,
        Brick,
        Stone
    }

    public enum Size
    {
        None,
        Fine,
        Diminutive,
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan,
        Colossal
    }
}