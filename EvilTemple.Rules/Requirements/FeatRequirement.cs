#region

using EvilTemple.Rules.Feats;

#endregion

namespace EvilTemple.Rules.Requirements
{
    public class FeatRequirement : IRequirement
    {
        public bool SameArguments { get; set; }

        public FeatInstance Instance { get; set; }

        public bool Satisfied(Critter critter, object context = null)
        {
            // TODO: Implement
            return false;
        }

        public string ShortDescription
        {
            get { return Instance.ShortDescription; }
        }
    }
}