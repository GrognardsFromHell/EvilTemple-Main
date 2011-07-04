using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Runtime;

namespace EvilTemple.Rules
{
    public enum HpOnLevelUpSetting
    {
        /// <summary>
        /// Rolls the hit die on every level up except the first (where full HP are awarded).
        /// </summary>
        StandardRules,

        /// <summary>
        /// Always award the maximum number of hit points.
        /// </summary>
        Maximum,

        /// <summary>
        /// Award 1/2 times the maximum plus 1.
        /// </summary>
        RpgaRules,

    }

    public static class RulesSettings
    {

        public static HpOnLevelUpSetting HpOnLevelUpSetting
        {
            get { return Services.UserSettings.GetOrSet("HpOnLevelUpSetting", HpOnLevelUpSetting.StandardRules); }
        }

    }
}
