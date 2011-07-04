using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using EvilTemple.D20Rules;
using EvilTemple.Rules;
using EvilTemple.Rules.Feats;
using EvilTemple.Rules.Prototypes;
using EvilTemple.Rules.Requirements;
using EvilTemple.Support;
using Gui;
using Rules;
using EvilTemple.Runtime;

namespace EvilTemple.Gui
{
    public class CreateCharacter : Menu
    {
        private const float DefaultHeightFactor = 0.5f;
        
        private readonly Races _races;
        
        private readonly EquipmentStyles _equipmentStyles;
        
        private readonly Prototypes _prototypes;

        private readonly HairStyles _hairStyles;

        private readonly Deities _deities;

        private readonly CharacterClasses _classes;

        private readonly Domains _domains;

        private readonly FeatRegistry _feats;

        private readonly IUserInterface _userInterface;

        private readonly Skills _skills;

        private readonly Portraits _portraits;

        private readonly PlayerVoices _playerVoices;
        
        public CreateCharacter(Races races, EquipmentStyles equipmentStyles,
            Prototypes prototypes, HairStyles hairStyles, CharacterClasses classes, Deities deities,
            Domains domains, FeatRegistry feats, IUserInterface userInterface, Skills skills,
            Portraits portraits, PlayerVoices playerVoices) {
            _feats = feats;
            _races = races;
            _hairStyles = hairStyles;
            _equipmentStyles = equipmentStyles;
            _prototypes = prototypes;
            _classes = classes;
            _deities = deities;
            _domains = domains;
            _userInterface = userInterface;
            _skills = skills;
            _portraits = portraits;
            _playerVoices = playerVoices;
            }

        public event Action OnSuccess;

        public event Action OnCancel;

        private delegate void StatsDistributedAction(int str, int dex, int con, int intl, int wis, int cha);

        private PlayerCharacter currentCharacter = new PlayerCharacter();

        private CharacterClass _currentClass;

        private Alignment _partyAlignment;

        private List<FeatInstance> currentFeats;

        private IDictionary<Skill, int> _currentSkillDistribution;

        public void Show(Alignment partyAlignment)
        {
            _partyAlignment = partyAlignment;

            CurrentMenu = _userInterface.AddWidget("interface/CreateCharacter.qml");

            CurrentMenu.cancel += (Action)Cancel;

            CurrentMenu.activeStageRequested += (Action<int>)ActiveStageRequested;
            CurrentMenu.statsDistributed += (StatsDistributedAction)StatsDistributed;
            CurrentMenu.raceChosen += (Action<string>)RaceChosen;
            CurrentMenu.genderChosen += (Action<string>)GenderChosen;
            CurrentMenu.heightChosen += (Action<float>)HeightChosen;
            CurrentMenu.classChosen += (Action<string>)ClassChosen;
            CurrentMenu.alignmentChosen += (Action<string>)AlignmentChosen;
            CurrentMenu.deityChosen += (Action<string>)DeityChosen;
            
            // Start with the stats page
            CurrentMenu.overallStage = Stage.Stats;
            CurrentMenu.activeStage = Stage.Stats;

            currentCharacter = new PlayerCharacter();
            currentCharacter.Id = Guid.NewGuid().ToString();
            currentFeats = new List<FeatInstance>();
            _currentSkillDistribution = new Dictionary<Skill, int>();

            var featDialog = CurrentMenu.getFeatsDialog();
            featDialog.helpRequested += (Action<string>)ShowFeatHelp;
            featDialog.featAddRequested += (Action<string>)SelectFeat;
            featDialog.featRemoveRequested += (Action<int>)UnselectFeat;

            var skillsDialog = CurrentMenu.getSkillsDialog();
            skillsDialog.requestDetails += (Action<string, IDictionary<string, int>>)RequestSkillDetails;
            skillsDialog.skillPointDistributionChanged += (Action)SkillPointDistributed;

            var portraitDialog = CurrentMenu.getPortraitDialog();
            portraitDialog.selectedPortraitChanged += (Action)PortraitChosen;

            var voiceAndNameDialog = CurrentMenu.getVoiceAndNameDialog();
            voiceAndNameDialog.requestVoiceSample += (Action<string>)RequestVoiceSample;
            voiceAndNameDialog.nameChanged += (Action)NameChosen;
            voiceAndNameDialog.selectedVoiceChanged += (Action)VoiceChosen;
        }

        private void PortraitChosen()
        {
            var portraitDialog = CurrentMenu.getPortraitDialog();
            string portraitId = portraitDialog.selectedPortrait;

            Trace.TraceInformation("Set portrait to " + portraitId);
            currentCharacter.Portrait = _portraits[portraitId];
            CurrentMenu.largePortrait = currentCharacter.Portrait.LargeImage;
            CurrentMenu.overallStage = Stage.VoiceAndName;
        }

        private void RequestVoiceSample(string voiceId)
        {
            var sound = _playerVoices[voiceId].GetGeneric(GenericPhrase.Acknowledge);
            if (sound != null)
            {
                var audioEngine = Services.Get<IAudioEngine>();
                audioEngine.PlaySoundOnce(sound.Sound, SoundCategory.Interface);
            }
        }

        private void NameChosen()
        {
            var voiceAndNameDialog = CurrentMenu.getVoiceAndNameDialog();
            currentCharacter.IndividualName = voiceAndNameDialog.name;
            Trace.TraceInformation("Setting name to " + currentCharacter.IndividualName);

            if (currentCharacter.Voice != null && !string.IsNullOrWhiteSpace(currentCharacter.IndividualName))
                CurrentMenu.overallStage = Stage.Finished;
            else
                CurrentMenu.overallStage = Stage.VoiceAndName;
        }

        private void VoiceChosen()
        {
            var voiceAndNameDialog = CurrentMenu.getVoiceAndNameDialog();
            currentCharacter.Voice = _playerVoices[voiceAndNameDialog.selectedVoice];
            Trace.TraceInformation("Setting voice to " + currentCharacter.Voice.Id);

            if (currentCharacter.Voice != null && !string.IsNullOrWhiteSpace(currentCharacter.IndividualName))
                CurrentMenu.overallStage = Stage.Finished;
            else
                CurrentMenu.overallStage = Stage.VoiceAndName;
        }

        private void RequestSkillDetails(string skillId, IDictionary<string, int> skills)
        {
            var skillsDialog = CurrentMenu.getSkillsDialog();

            var skill = _skills[skillId];
            var rank = skills[skillId] / 2;
            skillsDialog.detailsRank = rank;
            switch (skill.Ability) {
                case AbilityScore.Strength:
                    skillsDialog.detailsAbility = "STR";
                    break;
                case AbilityScore.Dexterity:
                    skillsDialog.detailsAbility = "DEX";
                    break;
                case AbilityScore.Constitution:
                    skillsDialog.detailsAbility = "CON";
                    break;
                case AbilityScore.Intelligence:
                    skillsDialog.detailsAbility = "INT";
                    break;
                case AbilityScore.Wisdom:
                    skillsDialog.detailsAbility = "WIS";
                    break;
                case AbilityScore.Charisma:
                    skillsDialog.detailsAbility = "CHA";
                    break;
            }

            skillsDialog.detailsAbilityMod = currentCharacter.GetAbilityModifier(skill.Ability);

            var skillRanks = new Dictionary<Skill, int>();
            foreach (var k in _skills.Objects.Values) {
                skillRanks[k] = skills[k.Id] / 2;
            }
            var miscMod = skill.GetSynergyBonus(skillRanks); // TODO: Synergy and other bonuses

            skillsDialog.detailsMiscMod = miscMod;
            skillsDialog.detailsTotalMod = rank + miscMod + currentCharacter.GetAbilityModifier(skill.Ability);
        }

        private void SkillPointDistributed()
        {
            var skillsDialog = CurrentMenu.getSkillsDialog();
            if (skillsDialog.remainingSkillPoints == 0)
            {
                IDictionary<string, object> distribution = skillsDialog.skillPointDistribution;
                _currentSkillDistribution = new Dictionary<Skill, int>();
                foreach (var entry in distribution)
                    _currentSkillDistribution[_skills[entry.Key]] = Convert.ToInt32(entry.Value); // Finalize only when finished

                // TODO: Go to spells selection if class has selectable spells (memorization isn't done here)
                CurrentMenu.overallStage = Stage.Portrait;
            }
        }

        private void ShowFeatHelp(string featId)
        {
            // TODO: Implement
            Trace.TraceInformation("Showing help for feat " + featId);
        }

        private void LoadPortraits()
        {
            var portraitDialog = CurrentMenu.getPortraitDialog();
            portraitDialog.portraits = from p in _portraits.Objects.Values
                                       where p.Gender != Gender.Other && p.Race != null
                                       select ConvertPortrait(p);
            portraitDialog.playerRace = currentCharacter.Race.Id;
            portraitDialog.playerGender = currentCharacter.Gender.ToLegacyString();
            portraitDialog.selectedPortrait = currentCharacter.Portrait.Id;
        }

        private ExpandoObject ConvertPortrait(Portrait portrait)
        {
            dynamic result= new ExpandoObject();
            result.id = portrait.Id;
            result.gender = portrait.Gender.ToLegacyString();
            result.race = portrait.Race;
            result.large = portrait.LargeImage;
            result.medium = portrait.MediumImage;
            result.small = portrait.SmallImage;
            return result;
        }

        /**
         * Applies the values in currentSkillDistribution to the current character.
         * This is non-reversible.
         */
        private void CommitSkills() {
            foreach (var k in _currentSkillDistribution)
            {
                var rank = currentCharacter.GetSkillRank(k.Key);
                rank += k.Value;
                currentCharacter.SetSkillRank(k.Key, rank);
            }
        }

        private void FinishCharCreation()
        {
            CommitSkills();

            // Finish building the character and put it into the vault
            var vault = Services.Get<CharacterVault>();
            vault.Add(currentCharacter);

            // Close the dialog
            CurrentMenu = null;

            var handler = OnSuccess;

            OnSuccess = null;
            OnCancel = null;

            if (handler != null)
                handler();
        }

        private void LoadVoiceAndName()
        {
            var voiceAndNameDialog = CurrentMenu.getVoiceAndNameDialog();
            voiceAndNameDialog.name = currentCharacter.Name ?? "";
            voiceAndNameDialog.selectedVoice = currentCharacter.Voice != null ? currentCharacter.Voice.Id : null;

            var matchingVoices = _playerVoices.Objects.Values.Where(v => v.Gender == currentCharacter.Gender);
            voiceAndNameDialog.voices = matchingVoices.Select(ConvertVoice);
        }

        private static ExpandoObject ConvertVoice(PlayerVoice v)
        {
            dynamic result = new ExpandoObject();
            result.id = v.Id;
            result.name = v.Name;
            return result;
        }
        
        /**
         * Gets the arguments of a feat that are still available.
         * @param feat The feat object.
         */
        private IEnumerable<IDictionary<string, object>> GetAvailableFeatOptions(Feat feat) {

            // Filter out all the options that were already taken by the character.
            var availableOptions = feat.Parameter.Values.Where(o => !currentCharacter.HasFeat(feat, o.Key));

            // Now disable those options that have requirements.)
            return availableOptions.Select(o => {
                var requires = "";

                // Go up to the feat and check requirements that are *specific* to this option
                var allRequirementsMet = feat.Requirements.All(r => {
                    var cr = r as ConditionalRequirement;
                    if (cr.Condition == o.Key) {
                        // Applies to this option only
                        if (requires != "")
                            requires += ", ";
                        
                        requires += cr.Requirement.ShortDescription;

                            if (!cr.Requirement.Satisfied(currentCharacter, o.Key)) {
                                return false;
                            }
                    }

                    return true;
                });

                dynamic result = new ExpandoObject();
                result.id = o.Key;
                result.text = o.Value;
                result.requires = requires;
                result.disabled = !allRequirementsMet;
                return (IDictionary<string, object>)result;
            });
        }

        private bool AllFeatsTaken()
        {
            return GetNumberOfRemainingBonusFeats() == 0 && GetNumberOfRemainingFeats() == 0;
        }

        private void SelectFeat(string featId)
        {
            Trace.TraceInformation("Selecting feat " + featId);

            var feat = _feats[featId];

            if (feat.Parameter != null) {

                var dialog = _userInterface.AddWidget(("CreateCharacterFeatArgument.qml"));
                dialog.headline = feat.Parameter.Description;
                dialog.availableOptions = GetAvailableFeatOptions(feat);
                dialog.accepted += (Action<string>)(optionId => {
                    dialog.deleteLater();

                    var featInstance = new FeatInstance(featId, optionId);

                    // Finish the argument selection
                    currentFeats.Add(featInstance);
                    currentCharacter.AddFeat(featInstance);
                    LoadFeats();
                });
                dialog.cancelled += dialog.deleteLater();
            } else {
                var featInstance = new FeatInstance(featId);
                currentFeats.Add(featInstance);
                currentCharacter.AddFeat(featInstance);
                LoadFeats();
            }

            if (AllFeatsTaken()) {
                CurrentMenu.overallStage = Stage.Skills;
            }
        }

        private void UnselectFeat(int index)
        {
            var featInstance = currentFeats[index];

            Trace.TraceInformation("Deselecting feat " + featInstance);
            currentCharacter.RemoveFeat(featInstance);
            currentFeats.RemoveAt(index);
            LoadFeats();

            if (!AllFeatsTaken()) {
                CurrentMenu.overallStage = Stage.Feats;
            }
        }

        private void LoadRaces()
        {
            var races = new List<Race>(_races.Objects.Values);
            // Sort by name
            races.Sort((a, b) => a.Name.Translate().ToLower().CompareTo(b.Name.Translate().ToLower()));

            CurrentMenu.races = races.Select(r => r.ToPropertyMap());
        }

        private void LoadAlignments()
        {
             // Get allowable alignments (from the party alignment and the class)
            var alignments = new List<Alignment>(Alignments.CompatibilityTable[_partyAlignment]);

            foreach (var requirement in _currentClass.Requirements)
            {
                if (requirement is AlignmentRequirement)
                {
                    var alignmentRequirement = requirement as AlignmentRequirement;

                    for (var i = 0; i < alignments.Count; ++i)
                    {
                        if (alignmentRequirement.Inclusive.Count > 0 
                            && !alignmentRequirement.Inclusive.Contains(alignments[i])
                            || alignmentRequirement.Exclusive.Count > 0
                            && alignmentRequirement.Exclusive.Contains(alignments[i]))
                        {
                            alignments.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            CurrentMenu.availableAlignments = alignments.Select(a => a.ToLegacyString());
        }

        private void LoadDeities()
        {
            var deities = (from d in _deities.Objects.Values
                           where d.Alignment.IsCompatibleWith(currentCharacter.Alignment)
                           select d).ToList();

            deities.Sort((a,b) => a.Name.Translate().ToLower().CompareTo(b.Name.Translate().ToLower()));

            CurrentMenu.availableDeities = deities.Select(d => new Dictionary<string, object>
                                                                   {
                                                                       {"id", d.Id},
                                                                       {"name", d.Name.Translate()}
                                                                   });
        }

        private void LoadClasses()
        {
            var classes = new List<CharacterClass>(_classes.Objects.Values);
            
            // Sort by name
            classes.Sort((a, b) => a.Name.Translate().ToLower().CompareTo(b.Name.Translate().ToLower()));

            CurrentMenu.classes = classes.Select(r => r.ToPropertyMap());
        }

        private void StatsDistributed(int str, int dex, int con, int intl, int wis, int cha)
        {
            currentCharacter.BaseStrength = str;
            currentCharacter.BaseDexterity = dex;
            currentCharacter.BaseConstitution = con;
            currentCharacter.BaseIntelligence = intl;
            currentCharacter.BaseWisdom = wis;
            currentCharacter.BaseCharisma = cha;

            // Every stat must be > 0 for the entire distribution to be valid.
            var valid = str > 0 && dex > 0 && con > 0 && intl > 0 && wis > 0 && cha > 0;

            CurrentMenu.overallStage = valid ? Stage.Race : Stage.Stats;

            UpdateCharacterSheet();
        }
        
        private void RaceChosen(string raceId)
        {
            currentCharacter.Race = _races[raceId];

            CurrentMenu.overallStage = Stage.Gender;

            UpdatePrototype();
            UpdateModelViewer();
            if (currentCharacter.Gender != Gender.Other)
                HeightChosen(DefaultHeightFactor); // Default height
        }

        private void GenderChosen(string genderId)
        {
            currentCharacter.Gender = Genders.ParseLegacyString(genderId);

            UpdatePrototype();
            HeightChosen(DefaultHeightFactor); // Default height
            UpdateModelViewer();
        }

        private void HeightChosen(float heightFactor)
        {
            var race = currentCharacter.Race;

            var visuals = (currentCharacter.Gender == Gender.Female) ? race.FemaleCharacteristics : race.MaleCharacteristics;
            
            currentCharacter.Height = visuals.Height.Interpolate(heightFactor); // This will also affect rendering-scale.
            currentCharacter.Weight = visuals.Weight.Interpolate(heightFactor);

            // TODO: This formula is most likely wrong.
            /*
             Attempt at fixing this:
             Assume that the 0cm is scale 0 and the medium height between min/max (0.5f) is scale 1
             So for a height-range of 100cm-200cm, with a default of 150, the scale-range would be
             0.66 + (1.33 - 0.66) * heightFactor, where 0.66 = 100/150 and 1.33 = 200/150
             */
            var midHeight = visuals.Height.Interpolate(DefaultHeightFactor);
            var minFac = visuals.Height.Min / midHeight;
            var maxFac = visuals.Height.Max / midHeight;

            var adjustedHeightFac = minFac + (maxFac - minFac) * heightFactor;

            var modelScale = currentCharacter.Scale / 100.0f * adjustedHeightFac;
            CurrentMenu.getModelViewer().modelScale = modelScale;
            Trace.TraceInformation("Set model-viewer scale to " + modelScale);

            // Height changing never changes the state unless it is to advance it
            if (CurrentMenu.overallStage < Stage.Hair)
                CurrentMenu.overallStage = Stage.Class;

            UpdateCharacterSheet();
        }
        
        private void DeityChosen(string deityId)
        {
            var deity = _deities[deityId];

            currentCharacter.Deity = deity;
            CurrentMenu.overallStage = Stage.Features;
        }

        private void AlignmentChosen(string alignmentId)
        {
            var alignment = Alignments.ParseLegacyString(alignmentId);
            currentCharacter.Alignment = alignment;
            CurrentMenu.overallStage = Stage.Deity;
        }

        private void ClassChosen(string classId)
        {
            var classObj = _classes[classId];
            
            Trace.TraceInformation("Chose class: " + classObj.Id);

            Trace.TraceInformation("Hit Die: " + classObj.GetHitDie(1).Maximum);

            // Remove all previous classes
            currentCharacter.ClearClassLevels();

            // Give the character a corresponding class level
            classObj.GiveClassLevel(currentCharacter);

            _currentClass = classObj;

            UpdateCharacterSheet();

            if (CurrentMenu.overallStage < Stage.Alignment)
                CurrentMenu.overallStage = Stage.Alignment;
        }
        
        private void ActiveStageRequested(int stage)
        {
            switch (stage)
            {
                case Stage.Race:
                    LoadRaces();
                    break;
                case Stage.Height:
                    SetHeightSpan();
                    break;
                case Stage.Hair:
                    // TODO: Load Hair Styles & Colors
                    break;
                case Stage.Class:
                    LoadClasses();
                    break;
                case Stage.Alignment:
                    LoadAlignments();
                    break;
                case Stage.Deity:
                    LoadDeities();
                    break;
                case Stage.Features:
                    LoadFeatures();
                    break;
                case Stage.Feats:
                    LoadFeats();
                    break;
                case Stage.Skills:
                    LoadSkills();
                    break;
                case Stage.Portrait:
                    LoadPortraits();
                    break;
                case Stage.VoiceAndName:
                    LoadVoiceAndName();
                    break;
                case Stage.Finished:
                    FinishCharCreation();
                    return;
            }

            CurrentMenu.activeStage = stage;
        }

        private void LoadFeatures()
        {
            if (_currentClass.Id == StandardClasses.Cleric) {
                var features = CurrentMenu.loadFeaturesPage("CreateCharacterDomains.qml");
                features.availableDomains = currentCharacter.Deity.Domains.Select(d => d.ToPropertyMap());
                features.domainsSelected += (Action<string[]>)(domains => {
                    if (domains.Length == 2)
                    {
                        var domainSet = new HashSet<Domain>();
                        foreach (var d in domains)
                            domainSet.Add(_domains[d]);

                        currentCharacter.Domains = domainSet;
                        CurrentMenu.overallStage = Stage.Feats;
                    } else {
                        CurrentMenu.overallStage = Stage.Features;
                    }
                });
            } else if (_currentClass.Id == StandardClasses.Ranger) {
                // TODO: Ranger favored enemy
                CurrentMenu.overallStage = Stage.Feats;
            } else if (_currentClass.Id == StandardClasses.Wizard) {
                // TODO: Wizard specialisation
                CurrentMenu.overallStage = Stage.Feats;
            } else {
                CurrentMenu.overallStage = Stage.Feats;
            }
        }

        private void SetHeightSpan()
        {
            var visuals = currentCharacter.Race.MaleCharacteristics;

            if (currentCharacter.Gender == Gender.Female)
                visuals = currentCharacter.Race.FemaleCharacteristics;

            CurrentMenu.minHeight = visuals.Height.Min;
            CurrentMenu.maxHeight = visuals.Height.Max;
        }
        
        
        /**
         * Indicates that a given feat can be chosen again by the user, even if it is unique.
         * For simple feats, this means the user has not yet chosen the feat, for feats with
         * an argument, it is checked that at least one option has not yet been chosen.
         *
         * @param feat The feat object to check against.
         */
        bool CanFeatBeTakenAgain(Feat feat)
        {
            if (feat.Parameter != null) {
                // This checks that at least one of the options for this feat was not yet taken.
                return feat.Parameter.Values.Any(option => !currentCharacter.HasFeat(feat, option.Key));
            } else {
                // This is the simple case, the feat itself is either in the list of selected feats or not.
                return !currentCharacter.HasFeat(feat);
            }
        }

        int GetNumberOfAvailableBonusFeats()
        {
            var classLevel = currentCharacter.GetClassLevel(_currentClass).Levels;

            return _currentClass.BonusFeats.Count(bonusFeatSelection => bonusFeatSelection.Level == classLevel);
        }

        int GetNumberOfAvailableFeats()
        {
            return currentCharacter.Race.StartingFeats;
        }

        private int GetNumberOfRemainingBonusFeats()
        {
            var remaining = GetNumberOfAvailableBonusFeats();

            remaining -= currentFeats.Count(IsBonusFeat);

            if (remaining < 0)
                remaining = 0;

            return remaining;
        }

        /**
         * Checks if the feat is a bonus-feat valid for the current selection.
         */
        bool IsBonusFeat(FeatInstance feat) {
            var classLevel = currentCharacter.GetClassLevel(_currentClass).Levels;
            return _currentClass.BonusFeats.Any(bfs => bfs.Level == classLevel && bfs.Contains(feat));
        }
        
        private int GetNumberOfRemainingFeats()
        {
            var remaining = GetNumberOfAvailableFeats();
            var remainingBonus = GetNumberOfAvailableBonusFeats();

            currentFeats.ForEach(fi => {
                // Bonus feats are deducted from the available bonus feats first.
                if (remainingBonus > 0 && IsBonusFeat(fi))
                    remainingBonus--;
                else
                    remaining--;
            });

            return remaining;
        }

        public void LoadFeats()
        {
            var feats = _feats.Objects.Values;
            var featsDialog = CurrentMenu.getFeatsDialog();

            var remainingBonusFeats = GetNumberOfRemainingBonusFeats();
            var remainingFeats = GetNumberOfRemainingFeats();

            featsDialog.remainingFeats = remainingFeats;
            featsDialog.remainingBonusFeats = remainingBonusFeats;

            /*
             Build the list of available feats.
             */
            var availableFeats = feats.Where(f => !f.Unique || CanFeatBeTakenAgain(f));

            featsDialog.availableFeats = availableFeats.Select(feat => {
                var bonusFeat = IsBonusFeat(new FeatInstance {FeatId = feat.Id});
                var disabled = remainingFeats == 0 && !(bonusFeat && remainingBonusFeats > 0);

                // This disables the extra hints about bonus-feats when no bonus feats remain to be selected
                if (remainingBonusFeats == 0)
                    bonusFeat = false;

                string requires = null;
                if (feat.Requirements.Count > 0) {
                    requires = "";

                    feat.Requirements.ForEach(requirement => {
                        // Skip the requirement if it's conditional.
                        if (requirement is ConditionalRequirement)
                            return;

                        if (requires != "")
                            requires += ", ";

                        var requirementMet = false;

                        if (feat.Parameter == null)
                        {
                            requirementMet = requirement.Satisfied(currentCharacter);
                        }
                        else
                        {
                            // If it is met for a single combination, the feat should still be shown
                            requirementMet =
                                feat.Parameter.Values.Any(v => requirement.Satisfied(currentCharacter, v.Value));
                        }

                        if (requirementMet)
                        {
                            requires += requirement.ShortDescription;
                        } else {
                            disabled = true;
                            requires += "<font color='#cc0000'>" + requirement.ShortDescription + "</font>";
                        }
                    });
                }

                dynamic result = new ExpandoObject();
                result.id = feat.Id;
                result.name = feat.Name;
                result.requires = requires;
                result.disabled = disabled;
                result.bonusFeat = bonusFeat;
                return result;
            });

            var selectedFeats = currentFeats.Select(fi =>
                                                        {
                                                            dynamic result = new ExpandoObject();
                                                            var feat = _feats[fi.FeatId];
                                                            result.id = feat.Id;
                                                            result.name = feat.Name;
                                                            return result;
                                                        });
            featsDialog.selectedFeats = selectedFeats;
        }

        private void LoadSkills()
        {
            var skillsDialog = CurrentMenu.getSkillsDialog();
            var classSkills = _currentClass.GetClassSkills(1);
            var ecl = currentCharacter.EffectiveCharacterLevel;

            var maxSkillPoints = _currentClass.GetSkillPoints(1) 
                + currentCharacter.GetBaseAbilityModifier(AbilityScore.Intelligence);

            if (maxSkillPoints < 1)
                maxSkillPoints = 1;
            if (currentCharacter.Race.Id == StandardRaces.Human)
                maxSkillPoints += 1;
            if (ecl == 1)
                maxSkillPoints *= 4;

            var remainingSkillPoints = maxSkillPoints;

            foreach (var k in _currentSkillDistribution) {
                remainingSkillPoints -= classSkills.Contains(k.Key) ? k.Value/2 : k.Value;
            }

            skillsDialog.availableSkillPoints = maxSkillPoints;
            skillsDialog.remainingSkillPoints = remainingSkillPoints;
            skillsDialog.skills = _skills.Objects.Values.Select(skill => {
                var classSkill = classSkills.Contains(skill);
                var rank = currentCharacter.GetSkillRank(skill);
                var currentRank = rank;
                if (_currentSkillDistribution.ContainsKey(skill))
                    currentRank += _currentSkillDistribution[skill];

                dynamic result = new ExpandoObject();
                result.id = skill.Id;
                result.name = skill.Name;
                result.rank = currentRank;
                result.classSkill = classSkill;
                result.minimumRank = rank;
                result.maximumRank = classSkill ? (ecl + 3) * 2 : (ecl + 3);
                return result;
            });
        }

        // Updates the prototype of the current character based on the chosen race and gender
        private void UpdatePrototype()
        {
            if (currentCharacter.Race == null || currentCharacter.Gender == Gender.Other)
                return;

            var characteristics = currentCharacter.Race.GetCharacteristics(currentCharacter.Gender);
            var prototypeId = characteristics.Prototype;

            Trace.TraceInformation("Setting prototype of new character to " + prototypeId);

            currentCharacter.Prototype = _prototypes[prototypeId];
        }

        private void UpdateModelViewer()
        {
            if (currentCharacter.Race == null || currentCharacter.Gender == Gender.Other)
                return;

            var modelViewer = CurrentMenu.getModelViewer();

            modelViewer.modelRotation = -120;

            var models = modelViewer.models;
            var modelInstance = modelViewer.modelInstance;
            modelInstance.model = models.load(currentCharacter.Model);
            
            Trace.TraceInformation("Loading model: " + currentCharacter.Model);

            var materials = modelViewer.materials;

            var equipmentBuilder = new CritterEquipmentBuilder(_equipmentStyles);
            equipmentBuilder.BuildEquipment(currentCharacter);

            modelInstance.clearAddMeshes();
            modelInstance.clearOverrideMaterials();
            
            foreach (var entry in equipmentBuilder.OverrideMaterials)
            {
                var material = materials.load(entry.Value);
                modelInstance.overrideMaterial(entry.Key, material);
            }

            foreach (var entry in equipmentBuilder.AddMeshes)
            {
                Trace.TraceInformation("Loading addmesh {0}", entry);
                var model = models.load(entry);
                modelInstance.addMesh(model);
            }

            foreach (var entry in equipmentBuilder.MaterialProperties)
            {
                Trace.TraceInformation("Setting material property {0} to {1}", entry.Key, entry.Value);
                dynamic value = entry.Value;
                modelInstance.setMaterialPropertyVec4(entry.Key, value);
            }
        }
        
        private void UpdateCharacterSheet()
        {
            dynamic charSheet = new ExpandoObject();
            charSheet.strength = currentCharacter.Strength;
            charSheet.dexterity = currentCharacter.Dexterity;
            charSheet.constitution = currentCharacter.Constitution;
            charSheet.intelligence = currentCharacter.Intelligence;
            charSheet.wisdom = currentCharacter.Wisdom;
            charSheet.charisma = currentCharacter.Charisma;

            charSheet.height = currentCharacter.Height;
            charSheet.weight = currentCharacter.Weight;

            charSheet.experience = currentCharacter.ExperiencePoints;
            charSheet.level = currentCharacter.EffectiveCharacterLevel;
            charSheet.hitPoints = currentCharacter.HitPoints;
            // charSheet.armorClass = currentCharacter.ArmorClass;
            charSheet.fortitudeSave = currentCharacter.FortitudeSaveBonus;
            charSheet.reflexSave = currentCharacter.ReflexSaveBonus;
            charSheet.willSave = currentCharacter.WillSaveBonus;
            charSheet.initiative = currentCharacter.InitiativeBonus;
            charSheet.speed = currentCharacter.LandSpeed;
            charSheet.meleeBonus = currentCharacter.MeleeAttackBonus;
            charSheet.rangedBonus = currentCharacter.RangedAttackBonus;

            CurrentMenu.characterSheet = charSheet;
        }

        private void Cancel()
        {
            CurrentMenu = null;
            var handler = OnCancel;
            if (handler != null) handler();
        }

        private static class Stage
        {
            public const int Stats = 0;
            public const int Race = 1;
            public const int Gender = 2;
            public const int Height = 3;
            public const int Hair = 4;
            public const int Class = 5;
            public const int Alignment = 6;
            public const int Deity = 7;
            public const int Features = 8;
            public const int Feats = 9;
            public const int Skills = 10;
            public const int Spells = 11;
            public const int Portrait = 12;
            public const int VoiceAndName = 13;
            public const int Finished = 14;
        }
    }
}
