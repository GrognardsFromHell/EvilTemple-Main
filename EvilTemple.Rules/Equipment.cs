#region

using System;
using System.Collections.Generic;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;

#endregion

namespace EvilTemple.Rules
{

    public class EquipmentStyles : Registry<EquipmentStyle>
    {

        public EquipmentStyle Naked
        {
            get { return this["naked"]; }
        }

    }

    public class EquipmentStyle : IIdentifiable
    {
        public string Id { get; set; }

        public Dictionary<string, EquipmentSubStyle> SubStyles { get; private set; }

        public EquipmentStyle()
        {
            SubStyles = new Dictionary<string, EquipmentSubStyle>();
        }
    }

    public class EquipmentSubStyle
    {
        public static readonly EquipmentSubStyle Default = new EquipmentSubStyle();

        public IList<string> Meshes { get; private set; }

        /// <summary>
        ///   Material replacement table. Maps from material slot names (HEAD, CHEST, etc.)
        ///   to material filenames.
        /// </summary>
        public IDictionary<string, string> Materials { get; private set; }

        public EquipmentSubStyle()
        {
            Meshes = new List<string>();
            Materials = new Dictionary<string, string>();
        }
    }
}