#region



#endregion

namespace EvilTemple.Rules.Requirements
{
    internal class WildShapeAbilityRequirement : IRequirement
    {
        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            get { return "Wildshape Ability"; }
        }
    }
}