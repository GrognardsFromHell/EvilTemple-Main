namespace EvilTemple.Rules
{
    /// <summary>
    /// Defines the slots on a critter that items can be equipped on.
    /// This is independent of the definition for items, since
    /// a "Ring" can always be equipped either on the left or right
    /// ring finger.
    /// In the old system, elements of this enumeration were identifed
    /// by the item inventory location id, if it was greater than 200.
    /// </summary>
    public enum CritterEquipmentSlot
    {
        Head,
        Neck,
        Hands,
        MainHand,
        OffHand,
        Armor,
        FirstRing,
        SecondRing,
        Feet,
        Projectile,
        Cloak,
        Robes,
        Shield,
        Wrists
    }
}