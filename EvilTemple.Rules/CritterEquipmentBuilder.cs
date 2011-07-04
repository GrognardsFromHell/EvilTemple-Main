using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace EvilTemple.Rules
{
    /// <summary>
    /// This class builds a list of equipment that should be displayed for a
    /// critter.
    /// </summary>
    public class CritterEquipmentBuilder
    {
        private readonly EquipmentStyles _equipmentStyles;
        
        public IDictionary<string, string> OverrideMaterials { get; private set; }
        
        public IList<string> AddMeshes { get; private set; }

        public IDictionary<string, object> MaterialProperties { get; private set; }
        
        public CritterEquipmentBuilder(EquipmentStyles equipmentStyles)
        {
            _equipmentStyles = equipmentStyles;
            OverrideMaterials = new Dictionary<string, string>();
            AddMeshes = new List<string>();
            MaterialProperties = new Dictionary<string, object>();
        }

        public void BuildEquipment(Critter critter)
        {
            var subStyleId = critter.EquipmentSubStyleId;

            if (!_equipmentStyles.Naked.SubStyles.ContainsKey(subStyleId))
                return;

            var nakedStyle = _equipmentStyles.Naked.SubStyles[subStyleId];
            ApplySubStyle(nakedStyle);

            // Apply Equipment Styles
            foreach (var entry in critter.Equipment)
            {
                var style = entry.Value.ItemPrototype.EquipmentStyle;
                if (style != null)
                    ApplySubStyle(style.SubStyles[subStyleId]);
            }

            HairSubStyle subStyle;
            if (critter.HairStyle.SubStyles.TryGetValue(subStyleId, out subStyle))
            {
                // TODO: Logic for selecting the right hair model based on the characters headwear
                if (subStyle.BigModel != null)
                    AddMeshes.Add(subStyle.BigModel);
                else if (subStyle.SmallModel != null)
                    AddMeshes.Add(subStyle.SmallModel);
                else if (subStyle.NoneModel != null)
                    AddMeshes.Add(subStyle.NoneModel);

                // Only set the haircolor if there's any actual hair
                var color = critter.HairColor;
                MaterialProperties["hairColor"] = new Vector4(color.RedF, color.GreenF, color.BlueF, 1);
            }
        }

        private void ApplySubStyle(EquipmentSubStyle subStyle)
        {
            foreach (var entry in subStyle.Materials)
                OverrideMaterials[entry.Key] = entry.Value;

            foreach (var mesh in subStyle.Meshes)
                AddMeshes.Add(mesh);
        }

    }
}
