#region

using EvilTemple.Rules;

#endregion

namespace Rules
{
    /// <summary>
    ///   Represents a reputation the party can gain for completing certain tasks.
    /// </summary>
    public class Reputation : IIdentifiable
    {
        public static readonly Reputation Default = new Reputation();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Effect { get; set; }
    }
}