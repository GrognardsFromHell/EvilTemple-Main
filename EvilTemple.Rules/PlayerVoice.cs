using System;
using System.Collections.Generic;
using EvilTemple.Rules.Utilities;
using Rules;

namespace EvilTemple.Rules
{
    public class PlayerVoice : IIdentifiable
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        public string BasePath { get; set; }

        private static readonly Random _soundRandom = new Random();

        public VoiceDefinition GetGeneric(GenericPhrase phrase)
        {
            var soundList = Generic[GetPhraseId(phrase)];

            if (soundList.Count == 0)
                return null;

            var selectedSound = soundList[_soundRandom.Next(soundList.Count)];

            var result = new VoiceDefinition();
            if (!string.IsNullOrEmpty(selectedSound.Sound))
                result.Sound = BasePath + selectedSound.Sound;
            result.Text = selectedSound.Text;
            
            return result;
        }

        public Dictionary<string, List<VoiceDefinition>> Generic { get; set; }

        public Dictionary<string, VoiceDefinition> Areas { get; set; }

        public Dictionary<string, VoiceDefinition> Scenery { get; set; }

        private string GetPhraseId(GenericPhrase genericPhrase)
        {
            switch (genericPhrase)
            {
                case GenericPhrase.Acknowledge:
                    return "acknowledge";
                case GenericPhrase.Deny:
                    return "deny";
                case GenericPhrase.Death:
                    return "death";
                case GenericPhrase.Encumbered:
                    return "encumbered";
                case GenericPhrase.CriticalHp:
                    return "criticalHp";
                case GenericPhrase.SeesDeath:
                    return "seesDeath";
                case GenericPhrase.Combat:
                    return "combat";
                case GenericPhrase.CriticalHitByParty:
                    return "criticalHitByPart";
                case GenericPhrase.CriticalHitOnParty:
                    return "criticalHitOnParty";
                case GenericPhrase.CriticalMissByParty:
                    return "criticalMissByParty";
                case GenericPhrase.FriendlyFire:
                    return "friendlyFire";
                case GenericPhrase.ValuableLoot:
                    return "valuableLoot";
                case GenericPhrase.BossMonster:
                    return "bossMonster";
                case GenericPhrase.Bored:
                    return "bored";
                default:
                    throw new ArgumentOutOfRangeException("genericPhrase");
            }
        }
    }

    public class PlayerVoices : Registry<PlayerVoice>
    {
        
    }

    public enum GenericPhrase
    {
        Acknowledge,
        Deny,
        Death,
        Encumbered,
        CriticalHp,
        SeesDeath,
        Combat,
        CriticalHitByParty,
        CriticalHitOnParty,
        CriticalMissByParty,
        FriendlyFire,
        ValuableLoot,
        BossMonster,
        Bored
    }

    public class VoiceDefinition
    {
        public string Text { get; set; }

        public string Sound { get; set; }
    }

}
