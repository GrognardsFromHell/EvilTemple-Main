
namespace EvilTemple.Rules
{
    /// <summary>
    ///   Represents a reusable icon for use by items in the inventory, loot and equipment window.
    /// </summary>
    public class InventoryIcon : IIdentifiable
    {
        /// <summary>
        ///   Id of the inventory icon.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///   Filename of the inventory icon used.
        /// </summary>
        public string Icon { get; set; }
    }
}