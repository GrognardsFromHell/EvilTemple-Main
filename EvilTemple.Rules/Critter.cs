#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Prototypes;
using EvilTemple.Rules.Utilities;
using EvilTemple.Runtime;
using Newtonsoft.Json;
using Rules;

#endregion

namespace EvilTemple.Rules
{
    public class Critter : BaseObject
    {
        private int? _baseCharisma;
        private int? _baseConstitution;
        private int? _baseDexterity;
        private int? _baseIntelligence;
        private int? _baseStrength;
        private int? _baseWisdom;
        private bool? _concealed;
        private uint? _experiencePoints;
        private Gender? _gender;
        private int? _height;
        private bool? _killsOnSight;
        private ushort? _movementRange;
        private Portrait _portrait;
        private Race _race;
        private byte? _runFactor;
        private int? _weight;
        private List<FeatInstance> _feats;
        private HairStyle _hairStyle;
        private Color3? _hairColor;
        private string _unknownName;
        private List<ClassLevel> _classLevels;
        private Alignment? _alignment;
        private Deity _deity;
        private bool _deityHasValue;
        private HashSet<Domain> _domains;
        private Dictionary<Skill, int> _skillRanks;
        private Dictionary<CritterEquipmentSlot, Item> _equipment;

        public Critter()
        {
            _skillRanks = new Dictionary<Skill, int>();
            _equipment = new Dictionary<CritterEquipmentSlot, Item>();
        }

        public override BaseObjectPrototype DefaultPrototype
        {
            get { return CritterPrototype.Default; }
        }

        public override BaseObjectPrototype Prototype
        {
            get { return base.Prototype; }
            set
            {
                if (!(value is CritterPrototype))
                    throw new ArgumentException("Can only set a CritterPrototype on Critters.");
                base.Prototype = value;
            }
        }

        public CritterPrototype CritterPrototype
        {
            get { return (CritterPrototype) Prototype; }
        }

        public string UnknownName
        {
            get { return _unknownName ?? CritterPrototype.UnknownName; }
            set { _unknownName = (value == CritterPrototype.UnknownName) ? null : value; }
        }

        public int BaseStrength
        {
            get { return _baseStrength ?? CritterPrototype.BaseStrength; }
            set { _baseStrength = (value == CritterPrototype.BaseStrength) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseStrength()
        {
            return _baseStrength.HasValue;
        }

        public int BaseDexterity
        {
            get { return _baseDexterity ?? CritterPrototype.BaseDexterity; }
            set { _baseDexterity = (value == CritterPrototype.BaseDexterity) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseDexterity()
        {
            return _baseDexterity.HasValue;
        }

        public int BaseConstitution
        {
            get { return _baseConstitution ?? CritterPrototype.BaseConstitution; }
            set { _baseConstitution = (value == CritterPrototype.BaseConstitution) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseConstitution()
        {
            return _baseConstitution.HasValue;
        }

        public int BaseIntelligence
        {
            get { return _baseIntelligence ?? CritterPrototype.BaseIntelligence; }
            set { _baseIntelligence = (value == CritterPrototype.BaseIntelligence) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseIntelligence()
        {
            return _baseIntelligence.HasValue;
        }

        public int BaseWisdom
        {
            get { return _baseWisdom ?? CritterPrototype.BaseWisdom; }
            set { _baseWisdom = (value == CritterPrototype.BaseWisdom) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseWisdom()
        {
            return _baseWisdom.HasValue;
        }

        public int BaseCharisma
        {
            get { return _baseCharisma ?? CritterPrototype.BaseCharisma; }
            set { _baseCharisma = (value == CritterPrototype.BaseCharisma) ? (int?) null : value; }
        }

        public bool ShouldSerializeBaseCharisma()
        {
            return _baseCharisma.HasValue;
        }

        public int Height
        {
            get { return _height ?? CritterPrototype.Height; }
            set { _height = (value == CritterPrototype.Height) ? (int?) null : value; }
        }

        public bool ShouldSerializeHeight()
        {
            return _height.HasValue;
        }

        public int Weight
        {
            get { return _weight ?? CritterPrototype.Weight; }
            set { _weight = (value == CritterPrototype.Weight) ? (int?) null : value; }
        }

        public bool ShouldSerializeWeight()
        {
            return _weight.HasValue;
        }

        public HairStyle HairStyle
        {
            get { return _hairStyle ?? CritterPrototype.HairStyle; }
            set { _hairStyle = (value == CritterPrototype.HairStyle) ? null : value; }
        }

        public bool ShouldSerializeHairStyle()
        {
            return _hairStyle != null;
        }

        public Color3 HairColor
        {
            get { return _hairColor ?? CritterPrototype.HairColor; }
            set { _hairColor = (value == CritterPrototype.HairColor) ? (Color3?) null : value; }
        }

        public bool ShouldSerializeHairColor()
        {
            return _hairColor.HasValue;
        }

        public HashSet<Domain> Domains
        {
            get { return _domains ?? CritterPrototype.Domains; }
            set { _domains = (value == CritterPrototype.Domains) ? null : value; }
        }

        public bool ShouldSerializeDomains()
        {
            return _domains != null;
        }

        public int Strength
        {
            get { return BaseStrength; }
        }

        public int Dexterity
        {
            get { return BaseDexterity; }
        }

        public int Constitution
        {
            get { return BaseConstitution; }
        }

        public int Intelligence
        {
            get { return BaseIntelligence; }
        }

        public int Wisdom
        {
            get { return BaseWisdom; }
        }

        public int Charisma
        {
            get { return BaseCharisma; }
        }
        
        public bool Concealed
        {
            get { return _concealed ?? CritterPrototype.Concealed; }
            set { _concealed = (value == CritterPrototype.Concealed) ? (bool?) null : value; }
        }

        public bool ShouldSerializeConcealed()
        {
            return _concealed.HasValue;
        }

        public ushort MovementRange
        {
            get { return _movementRange ?? CritterPrototype.MovementRange; }
            set { _movementRange = (value == CritterPrototype.MovementRange) ? (ushort?) null : value; }
        }

        public bool ShouldSerializeMovementRange()
        {
            return _movementRange.HasValue;
        }

        public byte RunFactor
        {
            get { return _runFactor ?? CritterPrototype.RunFactor; }
            set { _runFactor = (value == CritterPrototype.RunFactor) ? (byte?) null : value; }
        }

        public bool ShouldSerializeRunFactor()
        {
            return _runFactor.HasValue;
        }

        public bool KillsOnSight
        {
            get { return _killsOnSight ?? CritterPrototype.KillsOnSight; }
            set { _killsOnSight = (value == CritterPrototype.KillsOnSight) ? (bool?) null : value; }
        }
        
        public bool ShouldSerializeKillsOnSight()
        {
            return _killsOnSight.HasValue;
        }

        public uint ExperiencePoints
        {
            get { return _experiencePoints ?? CritterPrototype.ExperiencePoints; }
            set { _experiencePoints = (value == CritterPrototype.ExperiencePoints) ? (uint?) null : value; }
        }

        public bool ShouldSerializeExperiencePoints()
        {
            return _experiencePoints.HasValue;
        }

        public Portrait Portrait
        {
            get { return _portrait ?? CritterPrototype.Portrait; }
            set { _portrait = (value == CritterPrototype.Portrait) ? null : value; }
        }

        public bool ShouldSerializePortrait()
        {
            return _portrait != null;
        }

        public Alignment Alignment
        {
            get { return _alignment ?? CritterPrototype.Alignment; }
            set { _alignment = (value == CritterPrototype.Alignment) ? (Alignment?) null : value; }
        }

        public bool ShouldSerializeAlignment()
        {
            return _alignment.HasValue;
        }

        public Gender Gender
        {
            get { return _gender ?? CritterPrototype.Gender; }
            set
            {
                if (value == Gender) return;
                _gender = (value == CritterPrototype.Gender) ? (Gender?) null : value;
                InvokeOnPropertyChanged("Gender");
            }
        }

        public bool ShouldSerializeGender()
        {
            return _gender.HasValue;
        }

        public Race Race
        {
            get { return _race ?? CritterPrototype.Race; }
            set { _race = (value == CritterPrototype.Race) ? null : value; }
        }
        
        public bool ShouldSerializeRace()
        {
            return _race != null;
        }
        
        [JsonIgnore]
        public IList<FeatInstance> Feats
        {
            get { return _feats != null ? _feats.AsReadOnly() : CritterPrototype.Feats.AsReadOnly(); }
        }

        [JsonProperty(PropertyName = "feats")]
        internal List<FeatInstance> FeatsInternal
        {
            get { return _feats; }
            set { _feats = value; }
        }

        [JsonIgnore]
        public IDictionary<CritterEquipmentSlot, Item> Equipment
        {
            get { return new Dictionary<CritterEquipmentSlot, Item>(_equipment); }
        }

        [JsonProperty(PropertyName = "equipment")]
        internal Dictionary<CritterEquipmentSlot, Item> EquipmentInternal
        {
            get { return _equipment; }
            set { _equipment = value; }
        }

        public void AddFeat(FeatInstance featInstance)
        {
            // Copy-on-write
            if (_feats == null)
                _feats = new List<FeatInstance>(CritterPrototype.Feats);

            _feats.Add(featInstance);

            if (_feats == CritterPrototype.Feats)
                _feats = null;
        }

        public FeatInstance FindFeatInstance(Feat feat)
        {
            return _feats.FirstOrDefault(featInstance => featInstance.FeatId == feat.Id);
        }

        public void Equip(CritterEquipmentSlot slot, Item item)
        {
            // TODO: Check for existing item on slot
            _equipment[slot] = item;
            item.EquippedBy = this;
        }

        public string EquipmentSubStyleId
        {
            get
            {
                var raceId = "unknown";
                if (Race != null)
                    raceId = Race.EquipmentSubStyleId;

                string genderId;
                switch (Gender)
                {
                    case Gender.Female:
                        genderId = "female";
                        break;
                    case Gender.Male:
                        genderId = "male";
                        break;
                    case Gender.Other:
                        genderId = "other";
                        break;
                    default:
                        genderId = "other";
                        Trace.TraceWarning("Critter has invalid gender: {0}", Gender);
                        break;
                }

                return raceId + "-" + genderId;
            }
        }

        [JsonIgnore]
        public IList<ClassLevel> ClassLevels
        {
            get
            {
                var classLevels = _classLevels ?? CritterPrototype.ClassLevels;
                return classLevels.AsReadOnly();
            }
        }

        [JsonProperty(PropertyName = "classLevels")]
        public List<ClassLevel> ClassLevelsInternal
        {
            get { return _classLevels; }
            set { _classLevels = value; }
        }

        public int EffectiveCharacterLevel
        {
            get { return ClassLevels.Count; }
        }

        public int GetBaseAbilityScore(AbilityScore score)
        {
            switch (score)
            {
                case AbilityScore.Strength:
                    return BaseStrength;
                case AbilityScore.Dexterity:
                    return BaseStrength;
                case AbilityScore.Constitution:
                    return BaseConstitution;
                case AbilityScore.Wisdom:
                    return BaseWisdom;
                case AbilityScore.Intelligence:
                    return BaseIntelligence;
                case AbilityScore.Charisma:
                    return BaseCharisma;
                default:
                    throw new ArgumentOutOfRangeException("score");
            }
        }

        public int GetBaseAbilityModifier(AbilityScore score)
        {
            return ((GetBaseAbilityScore(score) - 10)/2);
        }

        public ClassLevel GetClassLevel(CharacterClass characterClass)
        {
            return ClassLevels.FirstOrDefault(cl => cl.CharacterClass == characterClass);
        }

        public void AddClassLevel(ClassLevel classLevel)
        {
            if (GetClassLevel(classLevel.CharacterClass) != null)
                throw new ArgumentException("Already has class levels for " + classLevel.CharacterClass);
            CopyClassLevels();
            _classLevels.Add(classLevel);
        }

        public void CopyClassLevels()
        {
            if (_classLevels == null)
                _classLevels = new List<ClassLevel>(ClassLevels);
        }

        public void ClearClassLevels()
        {
            _classLevels = null;
        }


        public int GetAbilityModifier(AbilityScore abilityScore)
        {
            // TODO: Implement
            return GetBaseAbilityModifier(abilityScore);
        }

        public int FortitudeSaveBonus
        {
            get
            {
                var result = 0;
                result += ClassLevels.Sum(cl => cl.CharacterClass.FortitudeSave[cl.Levels]);
                result += GetAbilityModifier(AbilityScore.Constitution);
                return result;
            }
        }

        public int ReflexSaveBonus
        {
            get
            {
                var result = 0;
                result += ClassLevels.Sum(cl => cl.CharacterClass.ReflexSave[cl.Levels]);
                result += GetAbilityModifier(AbilityScore.Dexterity);
                return result;
            }
        }

        public int WillSaveBonus
        {
            get
            {
                var result = 0;
                result += ClassLevels.Sum(cl => cl.CharacterClass.WillSave[cl.Levels]);
                result += GetAbilityModifier(AbilityScore.Wisdom);
                return result;
            }
        }

        public int InitiativeBonus
        {
            get { return GetAbilityModifier(AbilityScore.Dexterity); }
        }

        public uint LandSpeed
        {
            get { return Race == null ? 0 : Race.LandSpeed; }
        }

        public int BaseAttackBonus
        {
            get { return ClassLevels.Sum(cl => cl.CharacterClass.BaseAttackBonus[cl.Levels]); }
        }

        public int MeleeAttackBonus
        {
            get { return BaseAttackBonus + GetAbilityModifier(AbilityScore.Strength); }
        }

        public int RangedAttackBonus
        {
            get { return BaseAttackBonus + GetAbilityModifier(AbilityScore.Dexterity); }
        }

        public Deity Deity
        {
            get { return _deityHasValue ? _deity : CritterPrototype.Deity; }
            set
            {
                if (value == CritterPrototype.Deity)
                {
                    _deityHasValue = false;
                    _deity = null;
                }
                else
                {
                    _deity = value;
                    _deityHasValue = true;
                }
            }
        }

        public bool ShouldSerializeDeity()
        {
            return _deityHasValue;
        }

        public void ResetDeity()
        {
            _deityHasValue = false;
            _deity = null;
        }

        public bool HasFeat(Feat feat, string option = null)
        {
            return Feats.Any(f => f.FeatId == feat.Id && f.ParameterValue == option);
        }

        public bool RemoveFeat(FeatInstance featInstance)
        {
            // Copy-on-write
            if (_feats == null)
                _feats = new List<FeatInstance>(CritterPrototype.Feats);

            var result = _feats.Remove(featInstance);

            if (_feats == CritterPrototype.Feats)
                _feats = null;

            return result;
        }

        [JsonProperty(PropertyName = "skillRanks")]
        [JsonConverter(typeof(SkillRankConverter))]
        public Dictionary<Skill, int> SkillRanksInternal
        {
            get { return _skillRanks; }
            set { _skillRanks = value; }
        }

        public int GetSkillRank(Skill skill)
        {
            int ranks;
            if (_skillRanks.TryGetValue(skill, out ranks))
                return ranks;
            CritterPrototype.SkillRanks.TryGetValue(skill, out ranks);
            return ranks;
        }

        public void SetSkillRank(Skill skill, int rank)
        {
            int prototypeRank;
            CritterPrototype.SkillRanks.TryGetValue(skill, out prototypeRank);
            if (prototypeRank == rank)
                _skillRanks.Remove(skill);
            else
                _skillRanks[skill] = rank;
        }
    }
}