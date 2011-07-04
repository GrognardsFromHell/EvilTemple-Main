#region

using System;
using System.Collections.Generic;
using EvilTemple.Rules.Feats;
using Rules;

#endregion

namespace EvilTemple.Rules.Prototypes
{
    public class CritterPrototype : BaseObjectPrototype
    {
        public new static readonly CritterPrototype Default = new CritterPrototype();

        public CritterPrototype()
        {
            MovementRange = 250;
            RunFactor = 3;
            DrawBehindWalls = true;
            KillsOnSight = false;
            BaseStrength = 10;
            BaseDexterity = 10;
            BaseConstitution = 10;
            BaseIntelligence = 10;
            BaseWisdom = 10;
            BaseCharisma = 10;
            Model = "meshes/PCs/PC_Human_Male/PC_Human_Male.model";
            WalkAnimationSpeed = 1;
            RunAnimationSpeed = 1;
            Domains = new HashSet<Domain>();
            Feats = new List<FeatInstance>();
            ClassLevels = new List<ClassLevel>();
            SkillRanks = new Dictionary<Skill, int>();
            HairStyle = HairStyles.None;
        }
        
        public int BaseCharisma { get; set; }
        public int BaseConstitution { get; set; }
        public int BaseDexterity { get; set; }
        public int BaseIntelligence { get; set; }
        public int BaseStrength { get; set; }
        public int BaseWisdom { get; set; }
        public bool Concealed { get; set; }
        public uint ExperiencePoints { get; set; }
        public Gender Gender { get; set; }
        public int Height { get; set; }
        public bool KillsOnSight { get; set; }
        public Portrait Portrait { get; set; }
        public Race Race { get; set; }
        public byte RunFactor { get; set; }
        public int Weight { get; set; }
        public ushort MovementRange { get; set; }
        public int Age { get; set; }
        public HairStyle HairStyle { get; set; }
        public Color3 HairColor { get; set; }
        public string UnknownName { get; set; }

        /// <summary>
        ///   The melee range of the critter in feet.
        /// </summary>
        public int Reach { get; set; }

        /// <summary>
        ///   This describes the type of energy that clerics can convert their spells to.
        ///   It affects whether they turn or control undead and whether they get cure
        ///   or inflict wound spells when they convert their normal spells.
        /// </summary>
        public ConversionChoice ConversionChoice { get; set; }

        /// <summary>
        ///   Factor to be multiplied with the usual run animation speed.
        ///   Default is 1.
        /// </summary>
        public float RunAnimationSpeed { get; set; }

        /// <summary>
        ///   Factor to be multiplied with the usual walk animation speed.
        ///   Default is 1.
        /// </summary>
        public float WalkAnimationSpeed { get; set; }

        public Alignment Alignment { get; set; }

        public HashSet<Domain> Domains { get; private set; }

        public List<FeatInstance> Feats { get; private set; }

        public List<ClassLevel> ClassLevels { get; private set; }

        public Deity Deity { get; set; }

        public IDictionary<Skill, int> SkillRanks { get; private set; }

        public override BaseObject Instantiate()
        {
            return new Critter
                       {
                           Prototype = this
                       };
        }
    }
}