using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Rolls.Rites;
using RollerEngine.WeekPlan;

namespace RollerEngine.Character
{

    public class HatysBuffPlan
    {
        public NamelessBuff Nameless { get; set; }
    }

    public class WeekPlan
    {
        public HatysBuffPlan BuffPlan { get; set; }
        public List<WeeklyActivity> Activities { get; set; }
    }

    public partial class HatysParty
    {
        private readonly Dictionary<string, Build> _partyBuilds;
        private readonly Dictionary<string, HatysPartyMember> _party;

        private readonly IRollLogger _log;
        private readonly IRoller _roller;
        public Nameless Nameless { get; private set; }
        public Spirdon Spiridon { get; private set; }
        public Yoki Yoki { get; private set; }
        public Kurt Kurt { get; private set; }
        public Kinfolk1 Kinfolk1 { get; private set; }
        public Kinfolk2 Kinfolk2 { get; private set; }
        public Lynn Lynn { get; private set; }
        

        public Dictionary<string, Dictionary<string, int>> OriginalStats;

        public int CaernChannellingUsedTimes { get; set; }

        public HatysParty(Dictionary<string, Build> party, IRollLogger log, IRoller roller)
        {
            _partyBuilds = party;
            _party  = new Dictionary<string, HatysPartyMember>();

            _log = log;
            _roller = roller;
            Nameless = new Nameless(party["Krivda"], log, roller, this);
            _party.Add(Nameless.CharacterName, Nameless);

            Spiridon = new Spirdon(party["Keltur"], log, roller, this);
            _party.Add(Spiridon.CharacterName, Spiridon);

            Yoki = new Yoki(party["Alisa"], log, roller, this);
            _party.Add(Yoki.CharacterName, Yoki);

            Kurt = new Kurt(party["Urfin"], log, roller, this);
            _party.Add(Kurt.CharacterName, Kurt);

            Kinfolk1 = new Kinfolk1(party["NPC 1"], log, roller, this);
            _party.Add(Kinfolk1.CharacterName, Kinfolk1);

            Kinfolk2 = new Kinfolk2(party["NPC 2"], log, roller, this);
            _party.Add(Kinfolk2.CharacterName, Kinfolk2);

            Lynn = new Lynn(party["NPC 3"], log, roller, this);
            _party.Add(Lynn.CharacterName, Lynn);

            OriginalStats = new Dictionary<string, Dictionary<string, int>>();


            Spiridon.HasOpenedCaern = true;
            Yoki.HasSpecOnInstruction = true;
            Kinfolk1.HasSpecOnInstruction = true;
            Kinfolk2.HasSpecOnInstruction = true;

            Nameless.Self.HasAncestorVeneration = true;
        }

        public List<Build> Builds
        {
            get
            {
                return _partyBuilds.Select(kvp => kvp.Value).ToList();;
            }
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller roller)
        {
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddPermanentModifiers(party, log);
            return new HatysParty(party, log, roller);
        }

        public void WeeklyBuff(HatysBuffPlan wpBuffPlan)
        {
            //todo: rewrite

            //boost
            Nameless.WeeklyPreBoost(wpBuffPlan.Nameless);
            //Nameless can preboost Instruct and his Teacher's Ease will have -1 dc due to Persuasion
            Spiridon.WeeklyPreBoost(Build.Abilities.Empathy);
            Spiridon.WeeklyBoostSkill(Build.Abilities.Rituals);
            Nameless.WeeklyBoostSkill(wpBuffPlan.Nameless);
        }


        private void StoreOriginalValues(Build bld)
        {
            Dictionary<string, int> trait=new Dictionary<string, int>();
            OriginalStats.Add(bld.Name, trait);

            foreach (var traitKvp in bld.Traits)
            {
                trait.Add(traitKvp.Key, traitKvp.Value);
            }
        }

        private void ShowLearningResults()
        {
            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, "==> week summary:");

            foreach (var origKvp in OriginalStats)
            {
                var activeTraits = _partyBuilds.First(p => p.Value.Name.Equals(origKvp.Key)).Value.Traits;

                foreach (var activeTrait in activeTraits)
                {
                    int currTraitValue = activeTrait.Value;
                    int origTraitValue = 0;
                    if (origKvp.Value.ContainsKey(activeTrait.Key))
                    {
                        origTraitValue = origKvp.Value[activeTrait.Key];
                    }

                    if (currTraitValue != origTraitValue)
                    {
                        if (activeTrait.Key.Contains(Build.DynamicTraits.ExpirienceLearned))
                        {
                            string baseTrait = Build.DynamicTraits.GetBaseTrait(activeTrait.Key, Build.DynamicTraits.ExpirienceLearned);

                            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} advanced in learning trait {1}: he already learned {2}XP towards next rank!. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.ExpiriencePool))
                        {
                            string baseTrait = Build.DynamicTraits.GetBaseTrait(activeTrait.Key, Build.DynamicTraits.ExpiriencePool);

                            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} advanced in learning trait {1}: he has {2}XP in trait pool. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.RiteLearned))
                        {
                            string baseTrait = Build.DynamicTraits.GetBaseTrait(activeTrait.Key, Build.DynamicTraits.RiteLearned);

                            if (currTraitValue == Build.RiteAlreadyLearned && currTraitValue != origTraitValue)
                            {
                                //finished learning rite this week
                                _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} has learned rite {1}!", origKvp.Key, baseTrait));
                            }
                            else if (currTraitValue != Build.RiteAlreadyLearned)
                            {
                                _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} advanced in learning rite {1}: he already learned {2} successes!. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                            }
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.RitePool))
                        {
                            //skip it
                        }
                        else 
                        {
                            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} advanced in learning trait {1}: he increased trait value from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
                        }
                    }
                }
            }
        }

        public void StartScene()
        {
            CaernChannellingUsedTimes = 0;

            foreach (var build in _partyBuilds)
            {
                var bDcMods = build.Value.BonusDCModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var bDcMod in bDcMods)
                {
                    build.Value.BonusDCModifiers.Remove(bDcMod);
                }

                var dcMods = build.Value.DCModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var dcMod in dcMods)
                {
                    build.Value.DCModifiers.Remove(dcMod);
                }

                var bdpms = build.Value.BonusDicePoolModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var bdpm in bdpms)
                {
                    build.Value.BonusDicePoolModifiers.Remove(bdpm);
                }

                var tms = build.Value.TraitModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var tm in tms)
                {
                    build.Value.TraitModifiers.Remove(tm);
                }

                build.Value.AncestorsUsesLeft = 1;
            }


            //todo: loop it!
            OriginalStats.Clear();
            StoreOriginalValues(Nameless.Self);
            StoreOriginalValues(Spiridon.Self);
            StoreOriginalValues(Yoki.Self);
            StoreOriginalValues(Kurt.Self);
            StoreOriginalValues(Kinfolk1.Self);
            StoreOriginalValues(Kinfolk2.Self);

            Spiridon.WeeklyActions = 1;
            Nameless.WeeklyActions = 1;
            Kurt.WeeklyActions = 1;
            Yoki.WeeklyActions = 1;
            Kinfolk1.WeeklyActions = 1;
            Kinfolk2.WeeklyActions = 1;
            Lynn.WeeklyActions = 1;

            //todo: THAT IS A SOLID TRASH. REWRITE IF YOU SEE IT!
            Nameless.BoneRhythmsUsagesLeft = 2;
            string keyBoneRythmsLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned, RitesDictionary.Rites[Rite.BoneRhythms].Name);
            foreach (var build in _partyBuilds)
            {
                if (! build.Value.Name.Equals(Nameless.CharacterName))
                {
                    if (build.Value.Traits.ContainsKey(keyBoneRythmsLearned))
                    {
                        if (build.Value.Traits[keyBoneRythmsLearned] == Build.RiteAlreadyLearned)
                        {
                            _party[build.Value.Name].BoneRhythmsUsagesLeft = 2;
                        }
                    }
                }
            }
        }

        private static void AddPermanentModifiers(Dictionary<string, Build> result, IRollLogger log)
        {
            foreach (KeyValuePair<string, Build> buildKvp in result)
            {
                //spirit heritage
                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.SpiritHeritage] = 5;
                }

                //Ancestors
                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") || buildKvp.Key.Equals("NPC 3"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ancestors] = 5;
                }

                if (buildKvp.Key.Equals("Alisa"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ancestors] = 2;
                }

                if (buildKvp.Key.Equals("Urfin"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ancestors] = 1;
                }

                if (buildKvp.Key.Equals("Keltur"))
                {
                    CommonBuffs.ApplyMedicalBundle(buildKvp.Value, log);
                }

                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") || buildKvp.Key.Equals("Alisa") || buildKvp.Key.Equals("Urfin") || buildKvp.Key.Equals("NPC 3"))
                {
                    buildKvp.Value.CharacterClass = Build.Classes.Werewolf;

                    //Hatys
                    if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") ||
                        buildKvp.Key.Equals("Alisa") || buildKvp.Key.Equals("Urfin"))
                    {
                        CommonBuffs.ApplyHatysBuff(buildKvp.Value, log);
                    }
                }

                if (buildKvp.Key.Equals("NPC 1") || buildKvp.Key.Equals("NPC 2"))
                {
                    buildKvp.Value.CharacterClass = Build.Classes.Kinfolk;
                }

                if (buildKvp.Key.Equals("Ptitsa"))
                {
                    buildKvp.Value.CharacterClass = Build.Classes.Corax;
                }

                //Sprit heritages
                if (buildKvp.Value.Traits[Build.Backgrounds.SpiritHeritage] != 0)
                {
                    buildKvp.Value.BonusDicePoolModifiers.Add(new BonusModifier(
                        "Spirit Heritage",
                        DurationType.Permanent,
                        new List<string>() { Build.Conditions.SpiritHeritage },
                        buildKvp.Value.Traits[Build.Backgrounds.SpiritHeritage]
                    ));
                }
            }
        }

        public void DoWeek(int weekNo)
        {
            WeekPlan wp = GetPlanByWeekNumber(weekNo);
            List<WeeklyActivity> plan = wp.Activities;

            _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("<==== Week {0} starts", weekNo));

            StartScene();

            CheckFullAndCalcPartialActions(weekNo, plan); //CastMindPartition is inside

            //week1 dirty hack due to desync of automation and manual calculations !!!
            if (weekNo == 1)
            {
                Nameless.WeeklyPartialActions = 0;
                Spiridon.WeeklyPartialActions = 0;
                Kinfolk1.WeeklyPartialActions = 2;
                Kurt.WeeklyPartialActions = 2;
                Yoki.WeeklyPartialActions = 2;
                Kinfolk2.WeeklyPartialActions = 0;
            }

            WeeklyBuff(wp.BuffPlan);

            //first of all queue rites and roll teaching
            foreach (var planItem in plan)
            {
                switch (planItem.Activity)
                {
                    case Activity.TeachAbility:
                    {
                        TeachAbility weeklyActivity = (TeachAbility) planItem;
                        planItem.Actor.Instruct(weeklyActivity.Student.Self, weeklyActivity.Ability, false);
                        break;
                    }

                    case Activity.TeachGiftToGarou:
                    {
                        TeachGiftToGarou weeklyActivity = (TeachGiftToGarou) planItem;
                        //planItem.Actor.Instruct(weeklyActivity.Student.Self, weeklyActivity.Gift, false);
                        _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} teachs {1} for gift {2}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.Student.Self.Name,
                            weeklyActivity.Gift));
                        break;
                    }

                    case Activity.TeachRiteToGarou:
                    {
                        TeachRiteToGarou weeklyActivity = (TeachRiteToGarou)planItem;
                        _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} teachs {1} for Rite {2}",
                            weeklyActivity.Actor.CharacterName, 
                            weeklyActivity.Student.Self.Name,
                            RitesDictionary.Rites[weeklyActivity.Rite].Name));
                        break;
                    }

                    case Activity.CreateDevice:
                    {
                        CreateDevice weeklyActivity = (CreateDevice) planItem;
                        //planItem.Actor.Craft(weeklyActivity.Student.Self, weeklyActivity.FetishName);
                        _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} crafts device {1}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.DeviceName));
                            break;
                    }

                    case Activity.CreateFetishBase:
                    {
                        CreateFetishBase weeklyActivity = (CreateFetishBase) planItem;
                            //planItem.Actor.Craft(weeklyActivity.Student.Self, weeklyActivity.DeviceName);
                        _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} crafts item for fetish {1}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.FetishName));
                        break;
                    }

                    case Activity.QueueRiteLearning:
                    {
                        QueueRiteLearning weeklyActivity = (QueueRiteLearning) planItem;
                        RiteInfo riteInfo = RitesDictionary.Rites[weeklyActivity.Rite];
                        string keyRitePool = Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, riteInfo.Name);
                        string keyRiteLearned =
                            Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned, riteInfo.Name);

                        //create dynamic trait if it was absent
                        if (!planItem.Actor.Self.Traits.ContainsKey(keyRitePool))
                        {
                            planItem.Actor.Self.Traits.Add(keyRitePool, (int) (10.0 * riteInfo.Level));
                        }

                        //create dynamic trait if it was absent
                        if (!planItem.Actor.Self.Traits.ContainsKey(keyRiteLearned))
                        {
                            planItem.Actor.Self.Traits.Add(keyRiteLearned, 0);
                        }
                        break;
                    }
                }
            }

            //person spends learning attempts
            foreach (var actor in _party)
            {
                List<WeeklyActivity> characterActivities = WeeklyFilter.ByActor(plan, actor.Key);
                List<CreateTalens> createTalens = WeeklyFilter.ByCreateTalens(characterActivities); //talens are single
                List<CreateFetish> createFetish = WeeklyFilter.ByCreateFetish(characterActivities); //fetishes are single
                List<LearnAbility> learnAbility = WeeklyFilter.ByLearnAbility(characterActivities);
                List<LearnRiteFromGarou> learnRite = WeeklyFilter.ByLearnRiteFromGarou(characterActivities);
                List<LearnGiftFromGarou> learnGift = WeeklyFilter.ByLearnGiftFromGarou(characterActivities);

                //TODO: more checks about conflicting planItems

                foreach (var planItem in createFetish)
                {
                    //TODO: need IExtendedActinon aka dynamic trait
                    planItem.Actor.WeeklyPartialActions -= planItem.MaxCreationAttempts; //hardcoded to 1 in CreateFetish constructor
                    _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} creating fetish {1} ({2} attempts)",
                        planItem.Actor.CharacterName,
                        planItem.FetishName,
                        planItem.MaxCreationAttempts));
                }

                foreach (var planItem in createTalens)
                {
                    //TODO: need IExtendedActinon aka dynamic trait
                    planItem.Actor.WeeklyPartialActions -= planItem.MaxCreationAttempts; //limited to 2 in CreateTalens constructor
                    _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} creating talens {1} ({2} attempts)",
                        planItem.Actor.CharacterName,
                        planItem.TalenName,
                        planItem.MaxCreationAttempts));
                }

                /*
                foreach (var planItem in learnGift)
                {
                    planItem.Actor.AutoLearnRite(planItem.MaxLearnAttempts);
                }
                */

                foreach (var planItem in learnRite)
                {
                    planItem.Actor.AutoLearnRite(planItem.MaxLearnAttempts);
                }

                foreach (var planItem in learnAbility)
                {
                    planItem.Actor.AutoLearn(planItem.MaxLearnAttempts);
                }

                //TODO: need to have priority of learn types per character
                //actor.Value.AutoLearnGift(HatysPartyMember.SPEND_ALL_ATTEMPTS); //first try to learn more gifts
                actor.Value.AutoLearn(HatysPartyMember.SPEND_ALL_ATTEMPTS);     //first try to learn more traits
                actor.Value.AutoLearnRite(HatysPartyMember.SPEND_ALL_ATTEMPTS); //second try to learn more rites

                //TODO: maybe log attempts.
            }

            ShowLearningResults();

            _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("<==== Week {0} ends", weekNo));
        }

        private void CheckFullAndCalcPartialActions(int weekNo, List<WeeklyActivity> plan)
        {
            CheckFullAndCalcPartialActions(weekNo, plan, weekNo < 10);
        }

        private void CheckFullAndCalcPartialActions(int weekNo, List<WeeklyActivity> plan, bool bSerialArc6)
        {            
            //spiridon casts mind partition to increase base actions.
            Spiridon.CastMindPartition();

            foreach (var hatysPartyMemberKvp in _party)
            {
                var initialWeeklyActions = hatysPartyMemberKvp.Value.WeeklyActions;
                List<WeeklyActivity> characterActivities = WeeklyFilter.ByActor(plan, hatysPartyMemberKvp.Key);
                //var markerActivities = WeeklyFilter.ByMarker(characterActivities);
                var creationActivities = WeeklyFilter.ByCreation(characterActivities);
                var teachingActivities = WeeklyFilter.ByTeaching(characterActivities);                 //all single
                //var learningActivitires = WeeklyFilter.ByLearning(characterActivities);                //all extended
                var creationExtended = WeeklyFilter.ByType(creationActivities, ActivityType.Extended);
                //var creationSingle = WeeklyFilter.ByType(creationActivities, ActivityType.Single);

                var actions = hatysPartyMemberKvp.Value.WeeklyActions;
                if (creationExtended.Count > 1)
                {
                    throw new Exception(string.Format("No way to perform more than one extended creation roll! ({0})", hatysPartyMemberKvp.Key));
                }
                hatysPartyMemberKvp.Value.WeeklyActions -= creationExtended.Count;

                if (teachingActivities.Count > 1)
                {
                    throw new Exception(string.Format("Multiple teaching is not supported! ({0})", hatysPartyMemberKvp.Key));
                }
                hatysPartyMemberKvp.Value.WeeklyActions -= teachingActivities.Count;

                if (hatysPartyMemberKvp.Value.WeeklyActions < 0)
                {
                    throw new Exception(string.Format("Not enough weekly actions! ({0})", hatysPartyMemberKvp.Key));
                }

                if (hatysPartyMemberKvp.Key.Equals(Yoki.CharacterName))
                {
                    //yoki has x4 multiplier from Eidetic memory
                    hatysPartyMemberKvp.Value.WeeklyPartialActions = hatysPartyMemberKvp.Value.WeeklyActions * (bSerialArc6 ? 4 : 2);
                }
                else
                {
                    //all others use cacao
                    hatysPartyMemberKvp.Value.WeeklyPartialActions = hatysPartyMemberKvp.Value.WeeklyActions * 2;
                    //TODO: Spiridon if Learn was ended at first roll; then skip second roll (LATER)
                }

                _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} has {1} full spend {2} full and {3} partial actions.",
                    hatysPartyMemberKvp.Key,
                    initialWeeklyActions,
                    initialWeeklyActions - hatysPartyMemberKvp.Value.WeeklyActions,
                    hatysPartyMemberKvp.Value.WeeklyPartialActions));

            }

        }
    }

}