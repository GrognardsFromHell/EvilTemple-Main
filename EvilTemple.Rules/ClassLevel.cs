using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Rules
{
    public class ClassLevel
    {
        public static readonly ClassLevel Default = new ClassLevel();

        public ClassLevel()
        {
            HpGained = new List<int>();
        }

        public ClassLevel(CharacterClass characterClass) : this()
        {
            CharacterClass = characterClass;
        }

        public CharacterClass CharacterClass { get; set; }

        public int Levels { get; set; }

        public List<int> HpGained { get; private set; }
    }
}
