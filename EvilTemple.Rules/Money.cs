using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Rules
{
    public class Money
    {

        private int _value;

        public const int SilverInCopper = 10;
        public const int GoldInSilver = 10;
        public const int GoldInCopper = SilverInCopper * GoldInSilver;

        public int Copper
        {
            get
            {
                return _value % SilverInCopper;
            }
        }

        public int TotalCopper
        {
            get { return _value; }
        }

        public int Silver
        {
            get
            {
                var silver = _value / SilverInCopper;
                return silver % GoldInSilver;
            }
        }

        public int TotalSilver
        {
            get { return _value/SilverInCopper; }
        }

        public int Gold
        {
            get { return _value/GoldInCopper; }
        }

        public void AddGold(int amount)
        {
            _value += amount * GoldInCopper;
            if (_value < 0)
                _value = 0;
        }

        public void AddSilver(int amount)
        {
            _value += amount * SilverInCopper;
            if (_value < 0)
                _value = 0;
        }

        public void AddCopper(int amount)
        {
            _value += amount;
            if (_value < 0)
                _value = 0;
        }

        public override string ToString()
        {
            var result = "";

            if (Gold > 0)
                result += Gold + 'g';

            if (Silver > 0)
                result += Silver + 'g';

            if (Copper > 0 || result == "")
                result += Copper + 'c';

            return result;
        }
    }
}
