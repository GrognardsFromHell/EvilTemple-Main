namespace EvilTemple.Rules.Requirements
{
    public class TurnOrRebukeRequirement : IRequirement
    {
        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            get { return "Turn or Rebuke Undead"; }
        }
    }
}