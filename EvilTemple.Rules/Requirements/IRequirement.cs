namespace EvilTemple.Rules.Requirements
{
    public interface IRequirement
    {
        bool Satisfied(Critter critter, object context = null);

        string ShortDescription { get; }
    }
}