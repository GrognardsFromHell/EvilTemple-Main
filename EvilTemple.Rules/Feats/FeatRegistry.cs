#region

using System.Collections.Generic;
using EvilTemple.Rules.Requirements;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;

#endregion

namespace EvilTemple.Rules.Feats
{
    public class FeatRegistry : Registry<Feat>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.TypeMapping.Add(typeof (FeatRequirement), "FeatRequirement");
            serializer.TypeMapping.Add(typeof (ConditionalRequirement), "ConditionalRequirement");
            serializer.TypeMapping.Add(typeof (CasterLevelRequirement), "CasterLevelRequirement");
            serializer.TypeMapping.Add(typeof (ClassLevelRequirement), "ClassLevelRequirement");
            serializer.TypeMapping.Add(typeof (TurnOrRebukeRequirement), "TurnOrRebukeRequirement");
            serializer.TypeMapping.Add(typeof (BaseAttackBonusRequirement), "BaseAttackBonusRequirement");
            serializer.TypeMapping.Add(typeof (AbilityRequirement), "AbilityRequirement");
            serializer.TypeMapping.Add(typeof (CharacterLevelRequirement), "CharacterLevelRequirement");
            serializer.TypeMapping.Add(typeof (ProficientWithWeaponRequirement), "ProficientWithWeaponRequirement");
            serializer.TypeMapping.Add(typeof (WildShapeAbilityRequirement), "WildShapeAbilityRequirement");

            return serializer;
        }
    }

    public sealed class FeatNameComparer : IComparer<Feat>
    {
        public static readonly FeatNameComparer Instance = new FeatNameComparer();

        public int Compare(Feat x, Feat y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}