namespace EvilTemple.Rules.Requirements
{
    public class ProficientWithWeaponRequirement : IRequirement
    {
        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            // TODO: Implement
            get { return "Proficient with weapon"; }
        }
    }
}