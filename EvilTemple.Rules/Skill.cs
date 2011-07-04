using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Rules.Utilities;

namespace EvilTemple.Rules
{

    public class Skill : IIdentifiable, IEquatable<Skill>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AbilityScore Ability { get; set; }
        
        public bool Equals(Skill other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Skill)) return false;
            return Equals((Skill) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(Skill left, Skill right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Skill left, Skill right)
        {
            return !Equals(left, right);
        }

        public int GetSynergyBonus(Dictionary<Skill, int> skillRanks)
        {
            // TODO: Synergy bonuses
            return 0;
        }
    }

    public class Skills : Registry<Skill>
    {
        
    }

}
