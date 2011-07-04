
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Requirements;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace EvilTemple.Rules
{

    public class CharacterClasses : Registry<CharacterClass>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();

            serializer.Converters.Add(new IdentifiableConverter<Feat, FeatRegistry>());
            serializer.Converters.Add(new IdentifiableConverter<Skill, Skills>());
            serializer.Converters.Add(new IdentifiableConverter<BonusFeatList, BonusFeatLists>());
            serializer.Converters.Add(new IdentifiableConverter<IProgressionTable, ProgressionTables>());
            serializer.Converters.Add(new DiceConverter());

            serializer.TypeMapping = new TwoWayMap<Type, string>
                             {
                                 {typeof(AlignmentRequirement), "AlignmentRequirement"}
                             };
            
            return serializer;
        }
    }

    public class BonusFeatSelection
    {

        public int Level { get; set; }

        public List<BonusFeatList> FeatLists { get; private set; }

        public BonusFeatSelection()
        {
            FeatLists = new List<BonusFeatList>();
        }

        public bool Contains(FeatInstance featInstance)
        {
            return FeatLists.Any(fl => fl.Contains(featInstance));
        }

    }

    public class CharacterClass : IIdentifiable
    {
        public static readonly CharacterClass Default = new CharacterClass();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Dice HitDie { get; set; }

        public List<IRequirement> Requirements { get; private set; }

        public int SkillPoints { get; set; }

        public IProgressionTable BaseAttackBonus { get; set; }

        public IProgressionTable FortitudeSave { get; set; }

        public IProgressionTable ReflexSave { get; set; }

        public IProgressionTable WillSave { get; set; }

        public HashSet<Skill> ClassSkills { get; set; }

        public List<BonusFeatSelection> BonusFeats { get; private set; }

        public CharacterClass()
        {
            Requirements = new List<IRequirement>();
            ClassSkills = new HashSet<Skill>();
            BonusFeats = new List<BonusFeatSelection>();
        }

        /// <summary>
        /// Gets the hit die of this class for a given class level.
        /// </summary>
        /// <param name="level">The class level to get the hit die for.</param>
        virtual public Dice GetHitDie(int level)
        {
            return HitDie;
        }

        /// <summary>
        /// Gives a single level of this class to a critter.
        /// </summary>
        /// <param name="critter">The critter to give a class level to.</param>
        virtual public void GiveClassLevel(Critter critter)
        {
            Trace.TraceInformation("Giving level of {0} to {1}", Id, critter.Id);

            // Is this the first level overall?
            var isFirst = critter.EffectiveCharacterLevel == 0;
            var classLevel = critter.GetClassLevel(this);

            // Initialize the structure if necessary
            if (classLevel == null) {
                classLevel = new ClassLevel(this);
                critter.AddClassLevel(classLevel);
            }
            else
            {
                // Detach class level array from prototype
                critter.CopyClassLevels();
            }

            classLevel.Levels++;
            
            var hpGained = GetHpGained(isFirst, classLevel.Levels);

            var hpConBonus = critter.GetBaseAbilityModifier(AbilityScore.Constitution);

            // Record the HP progression (separate by con bonus and actual hit die)
            classLevel.HpGained.Add(hpGained);

            if (isFirst) {
                // Objects have a single hit point as the base if they have no classes. Overwrite this here.
                critter.HitPoints = (uint) (hpGained + hpConBonus);
            } else {
                critter.HitPoints += (uint) (hpGained + hpConBonus);
            }
        }

        protected virtual int GetHpGained(bool isFirst, int level)
        {
            var hitDie = GetHitDie(level);

            int hpGained;
            if (isFirst) 
            {
                // Grant full hp on first character level
                hpGained = hitDie.Maximum;
            }
            else
            {
                switch (RulesSettings.HpOnLevelUpSetting) {
                    case HpOnLevelUpSetting.StandardRules:
                        hpGained = hitDie.Roll();
                        break;
                    case HpOnLevelUpSetting.Maximum:
                        hpGained = hitDie.Maximum;
                        break;
                    case HpOnLevelUpSetting.RpgaRules:
                        hpGained = hitDie.Maximum / 2 + 1;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown HpOnLevelUpSetting: " +
                                                            RulesSettings.HpOnLevelUpSetting);
                }
            }
            return hpGained;
        }

        public virtual ISet<Skill> GetClassSkills(int classLevel)
        {
            return ClassSkills;
        }

        public virtual int GetSkillPoints(int classLevel)
        {
            return SkillPoints;
        }
    }
}
