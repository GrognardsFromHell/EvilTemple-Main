using System;
using System.Collections.Generic;
using Rules;

namespace EvilTemple.Rules
{

    public class Party
    {
        public static Party Current { get; set; }
        
        public Alignment Alignment { get; set; }

        public IList<PlayerCharacter> Players { get; private set; }

        public Money Money { get; private set; }

        public Party()
        {
            Players = new List<PlayerCharacter>();
            Money = new Money();
        }

        public void Add(PlayerCharacter player)
        {
            Players.Add(player);
        }
    }

}
