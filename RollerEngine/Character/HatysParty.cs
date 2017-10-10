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
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

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
        private const int ANCESTOR_VENERATION_DELAY = 4;
        private readonly Dictionary<string, Build> _partyBuilds;
        private readonly Dictionary<string, HatysPartyMember> _party;

        private readonly IRollLogger _log;
        private readonly IRollAnalyzer _roller;
        public Nameless Nameless { get; private set; }
        public Spirdon Spiridon { get; private set; }
        public Yoki Yoki { get; private set; }
        public Kurt Kurt { get; private set; }
        public Kinfolk1 Kinfolk1 { get; private set; }
        public Kinfolk2 Kinfolk2 { get; private set; }
        public Lynn Lynn { get; private set; }


        public Dictionary<string, Dictionary<string, int>> WeekStartingTraits { get; set; }
        public Dictionary<string, Dictionary<string, int>> RunStartTraits { get; set; }

        public HatysParty(Dictionary<string, Build> party, IRollLogger log, IRollAnalyzer roller)
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

            WeekStartingTraits = new Dictionary<string, Dictionary<string, int>>();
            RunStartTraits = new Dictionary<string, Dictionary<string, int>>();


            Yoki.HasSpecOnInstruction = true;
            Kinfolk1.HasSpecOnInstruction = true;
            Kinfolk2.HasSpecOnInstruction = true;

            Nameless.Self.HasAncestorVeneration = true;

            foreach (var hatysPartyMemberKvp in _party)
            {
                RunStartTraits[hatysPartyMemberKvp.Value.CharacterName] = CopyTraitValues(hatysPartyMemberKvp.Value.Self);
            }
        }

        public List<Build> Builds
        {
            get
            {
                return _partyBuilds.Select(kvp => kvp.Value).ToList();;
            }
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller rollInterface)
        {
            var roller = new RollAnalyzer(rollInterface);
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddPermanentModifiers(party, log);
            return new HatysParty(party, log, roller);
        }

        public void WeeklyBuff(HatysBuffPlan wpBuffPlan)
        {
            if (Spiridon.HasCarnyx)
            {
                if (!wpBuffPlan.Nameless.MainBuff.Trait.Equals(Build.Abilities.Instruction))
                {
                    throw new Exception("If we have carnyx - we should use IT!");
                }
                //boost
                Spiridon.WeeklyPreBoost(Build.Abilities.Rituals);
                Nameless.WeeklyPreBoost(wpBuffPlan.Nameless);
                Spiridon.WeeklyBoostSkill(Build.Abilities.Performance);
                Nameless.WeeklyBoostSkill(wpBuffPlan.Nameless);
            }
            else
            {
                //boost
                Nameless.WeeklyPreBoost(wpBuffPlan.Nameless);
                //Nameless can preboost Instruct and his Teacher's Ease will have -1 dc due to Persuasion
                Spiridon.WeeklyPreBoost(Build.Abilities.Empathy);
                Spiridon.WeeklyBoostSkill(Build.Abilities.Rituals);
                Nameless.WeeklyBoostSkill(wpBuffPlan.Nameless);
            }


        }


        private Dictionary<string, int> CopyTraitValues(Build bld)
        {
            Dictionary<string, int> traits=new Dictionary<string, int>();

            foreach (var traitKvp in bld.Traits)
            {
                traits.Add(traitKvp.Key, traitKvp.Value);
            }

            //todo: refac this crap
            foreach (var itemKvp in bld.Items)
            {
                //todo: PIZDA. A bad-ass hack
                traits.Add("item" + itemKvp.Key, itemKvp.Value);
            }

            return traits;
        }

        private Dictionary<string, ProgressSummary> GetProgress(Dictionary<string, Dictionary<string, int>> original)
        {
            Dictionary<string, ProgressSummary> progress = new Dictionary<string, ProgressSummary>();

            //for each character
            foreach (var characterKvp in original)
            {
                string characterName = characterKvp.Key;

                var charProgress = new ProgressSummary();
                progress.Add(characterName, charProgress);

                var origCharacterTraits = characterKvp.Value;
                var currCharacterTraits = _party[characterName].Self.Traits;

                int a = 0;

                foreach (var currTraitKvp in currCharacterTraits)
                {

                    a++;
                    string trait = currTraitKvp.Key;
                    Tuple<int, int> traitDiff = GetTraitDiff(trait, origCharacterTraits, currCharacterTraits);


                    //is ability?
                    //todo: PIIIIZZZDDDAAA
                    if (typeof(Build.Abilities).GetFields().Any(fi => fi.GetValue(null).ToString().Equals(currTraitKvp.Key)))
                    {
                        //calc progress in abilities

                        string traitLearnedKey = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, trait);
                        string traitPoolKey = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpiriencePool, trait);

                        Tuple<int, int> traitLearnedDiff =
                            GetTraitDiff(traitLearnedKey, origCharacterTraits, currCharacterTraits);
                        Tuple<int, int> traitPoolDiff =
                            GetTraitDiff(traitPoolKey, origCharacterTraits, currCharacterTraits);

                        if (
                            (traitDiff.Item1 != traitDiff.Item2) ||
                            (traitLearnedDiff.Item1 != traitLearnedDiff.Item2) ||
                            (traitPoolDiff.Item1 != traitPoolDiff.Item2))
                        {

                            var traitProgress = new Dictionary<string, Tuple<int, int>>
                            {
                                {trait, traitDiff},
                                {traitLearnedKey, traitLearnedDiff},
                                {traitPoolKey, traitPoolDiff}
                            };
                            charProgress.AbilityProgress.Add(trait, traitProgress);
                        }
                    }
                    //is rite pool learned?
                    else if (trait.Contains(Build.DynamicTraits.RiteLearned))
                    {
                        //calc progress in rites

                        string riteName = Build.DynamicTraits.GetBaseTrait(trait, Build.DynamicTraits.RiteLearned);

                        if (traitDiff.Item2 == Build.RiteAlreadyLearned && traitDiff.Item1 != traitDiff.Item2)
                        {
                            //finished learning rite this week
                            charProgress.RiteProgress.Add(riteName, new Tuple<int, int, int>(0, Build.RiteAlreadyLearned, Build.RiteAlreadyLearned));
                        }
                        else if (traitDiff.Item2 != Build.RiteAlreadyLearned && traitDiff.Item1 != traitDiff.Item2)
                        {
                            string ritePoolKey = Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, riteName);
                            int ritePool;
                            if (! currCharacterTraits.TryGetValue(ritePoolKey, out ritePool))
                            {
                                ritePool = 0;
                            }

                            //progressed in learning rite
                            charProgress.RiteProgress.Add(riteName, new Tuple<int, int, int>(traitDiff.Item1, traitDiff.Item2, ritePool));
                        }
                    }
                //traits
                }

                var character = _party[characterName];

                //items
                foreach (var item in character.Self.Items)
                {
                    int origItemCount;
                    //todo: PIZDA. A bad-ass hack
                    if (!origCharacterTraits.TryGetValue("item" + item.Key, out origItemCount))
                    {
                        //new item
                        charProgress.ItemsProgress.Add(item.Key, new Tuple<int,int>(0, item.Value));
                    }
                    else if (item.Value != origItemCount)
                    {
                        charProgress.ItemsProgress.Add(item.Key, new Tuple<int, int>(origItemCount, item.Value));
                    }
                }

            //characters
            }

            return progress;
        }

        private Tuple<int, int> GetTraitDiff(string trait, Dictionary<string, int> origCharacterTraits, Dictionary<string, int> currCharacterTraits)
        {

            int origValue;
            if (!origCharacterTraits.TryGetValue(trait, out origValue))
            {
                origValue = 0;
            }

            int currValue;
            if (!currCharacterTraits.TryGetValue(trait, out currValue))
            {
                origValue = 0;
            }

            return new Tuple<int, int>(origValue, currValue) ;
        }

        private void LogProgress(Dictionary<string, ProgressSummary> progress)
        {
            foreach (var charProgressKvp in progress)
            {
                string characterName = charProgressKvp.Key;

                foreach (var charAbilityProgress in charProgressKvp.Value.AbilityProgress)
                {
                    string trait = charAbilityProgress.Key;
                    var traitDiff = charAbilityProgress.Value[trait];
                    var traitLearnedDiff =
                        charAbilityProgress.Value[
                            Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, trait)];
                    var traitPoolDiff =
                        charAbilityProgress.Value[
                            Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpiriencePool, trait)];

                    _log.Log(Verbosity.Critical,
                        string.Format(
                            "{0} has progressed in learning ability {1}:trait value {2}->{3}, XP learned {4}->{5}, XP pool {6}->{7}",
                            characterName, trait, traitDiff.Item1, traitDiff.Item2, traitLearnedDiff.Item1,
                            traitLearnedDiff.Item2, traitPoolDiff.Item1, traitPoolDiff.Item2));
                }

                foreach (var riteProgressKvp in charProgressKvp.Value.RiteProgress)
                {
                    string riteName = riteProgressKvp.Key;
                    if (riteProgressKvp.Value.Item2 == Build.RiteAlreadyLearned)
                    {
                        //learned rite!
                        _log.Log(Verbosity.Critical, 
                            string.Format("{0} has learned rite {1}.", characterName, riteName));
                    }
                    else
                    {
                        _log.Log(Verbosity.Critical, 
                            string.Format("{0} has progressed in learning rite {1}: {2}->{3} of {4}.", characterName,
                                riteName,
                                riteProgressKvp.Value.Item1, riteProgressKvp.Value.Item2, riteProgressKvp.Value.Item3));
                    }
                }

                foreach (var itemKvp in charProgressKvp.Value.ItemsProgress)
                {
                    string direction;
                    //TODO: bug
                    if (itemKvp.Value.Item1 < itemKvp.Value.Item2)
                    {
                        direction = "got";
                    }
                    else
                    {
                        direction = "lost";
                    }

                    _log.Log(Verbosity.Critical, 
                        string.Format("{0} has {1} item {2}. Item count: {3}->{4}", characterName, direction,
                            itemKvp.Key, itemKvp.Value.Item1, itemKvp.Value.Item2));
                }
            }
        }

        public void LogTotalProgress()
        {
            var progress = GetProgress(RunStartTraits);
            _log.Log(Verbosity.Warning,  "");
            _log.Log(Verbosity.Warning,  "");
            _log.Log(Verbosity.Warning,  "===== >>>> This run results:");
            LogProgress(progress);
            _log.Log(Verbosity.Warning,  "<<<< =====");
        }

        public void StartScene(int weekNo)
        {

            foreach (var buildKvp in _partyBuilds)
            {
                var bDcMods = buildKvp.Value.BonusDCModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var bDcMod in bDcMods)
                {
                    buildKvp.Value.BonusDCModifiers.Remove(bDcMod);
                }

                var dcMods = buildKvp.Value.DCModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var dcMod in dcMods)
                {
                    buildKvp.Value.DCModifiers.Remove(dcMod);
                }

                var bdpms = buildKvp.Value.BonusDicePoolModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var bdpm in bdpms)
                {
                    buildKvp.Value.BonusDicePoolModifiers.Remove(bdpm);
                }

                var tms = buildKvp.Value.TraitModifiers.FindAll(m => m.Duration != DurationType.Permanent);
                foreach (var tm in tms)
                {
                    buildKvp.Value.TraitModifiers.Remove(tm);
                }

                if (buildKvp.Value.CharacterClass.Equals(Build.Classes.Werewolf) &&
                    buildKvp.Value.Traits[Build.Backgrounds.Ancestors]>0)
                {
                    buildKvp.Value.AncestorsUsesLeft = 1;
                }

            }

            WeekStartingTraits.Clear();
            foreach (var hatysPartyMemberKvp in _party)
            {
                WeekStartingTraits[hatysPartyMemberKvp.Value.CharacterName] = CopyTraitValues(hatysPartyMemberKvp.Value.Self);
                hatysPartyMemberKvp.Value.WeeklyActions = 1;
            }

            foreach (var buildKvp in _party)
            {
                if (buildKvp.Value.Self.Rites.ContainsKey(Rite.BoneRhythms))
                {
                    buildKvp.Value.BoneRhythmsUsagesLeft = 2;
                }

                if (buildKvp.Value.Self.Rites.ContainsKey(Rite.OpenedCaern))
                {
                    buildKvp.Value.HasOpenedCaern = true;
                }

                if (buildKvp.Value.Self.Rites.ContainsKey(Rite.AncestorVeneration))
                {
                    //enabled 4 weeks after learn
                    if (weekNo > buildKvp.Value.Self.Rites[Rite.AncestorVeneration] + ANCESTOR_VENERATION_DELAY)
                    {
                        buildKvp.Value.Self.HasAncestorVeneration = true;
                    }
                }
            }

            foreach (var buildKvp in _party)
            {
                if (buildKvp.Value.Self.CharacterClass.Equals(Build.Classes.Werewolf))
                {
                    //TODO check lupus & gift
                    if (!buildKvp.Key.Equals(Kurt.CharacterName))
                    {
                        buildKvp.Value.ShiftToCrinos();
                        //apply heighten sences
                        CommonBuffs.ApplyHeightenSenses(buildKvp.Value.Self, _log);
                    }
                }
            }
        }

        //Called ONCE after load from google!!!
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

                if (buildKvp.Key.Equals("Krivda"))
                {
                    buildKvp.Value.Rites.Add(Rite.AncestorVeneration, 0);
                    buildKvp.Value.Rites.Add(Rite.BoneRhythms, 0);
                }

                if (buildKvp.Key.Equals("Keltur"))
                {
                    CommonBuffs.ApplyMedicalBundle(buildKvp.Value, log);
                    buildKvp.Value.Rites.Add(Rite.OpenedCaern, 0);
                    //todo: very dirty hack
                    buildKvp.Value.Traits[Build.Abilities.VisageOfFenris] = 2;

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
            if (wp == null)
            {
                return;
            }

            List<WeeklyActivity> plan = wp.Activities;

            Nameless.BoostPlan = wp.BuffPlan.Nameless;

            _log.Week = weekNo;
            _log.Log(Verbosity.Critical,  string.Format("<==== Week {0} starts", weekNo));

            StartScene(weekNo);

            CheckFullAndCalcPartialActions(weekNo, plan); //CastMindPartition is inside

            //week1 dirty hack due to desync of automation and manual calculations !!!
            if (weekNo == 1)
            {
                Nameless.WeeklyPartialActions = 0;
                Spiridon.WeeklyPartialActions = 0;
                Kinfolk1.WeeklyPartialActions = 2;  //see Academic Plan
                Kurt.WeeklyPartialActions = 2;      //see Academic Plan
                Yoki.WeeklyPartialActions = 2;      //see Academic Plan
                Kinfolk2.WeeklyPartialActions = 0;
            }

            WeeklyBuff(wp.BuffPlan);

            //first of all queue rites and roll teaching
            foreach (var planItem in plan)
            {
                switch (planItem.Activity)
                {
                    case Activity.QueueRiteLearning:
                    {
                        planItem.Execute();
                        break;
                    }

                    case Activity.TeachAbility:
                    {
                        planItem.Execute();
                        break;
                    }

                    case Activity.TeachGiftToGarou:
                    {
                        TeachGiftToGarou weeklyActivity = (TeachGiftToGarou) planItem;
                        //planItem.Actor.Instruct(weeklyActivity.Student.Self, weeklyActivity.Gift, false);
                        _log.Log(Verbosity.Critical,  string.Format("{0} teachs {1} for gift {2}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.Student.Self.Name,
                            weeklyActivity.Gift));
                        break;
                    }

                    case Activity.TeachRiteToGarou:
                    {
                        TeachRiteToGarou weeklyActivity = (TeachRiteToGarou)planItem;
                        _log.Log(Verbosity.Critical,  string.Format("{0} teachs {1} for Rite {2}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.Student.Self.Name,
                            weeklyActivity.Rite.Info().Name));
                        break;
                    }

                    case Activity.CreateDevice:
                    {
                        CreateDeviceActivity weeklyActivity = (CreateDeviceActivity) planItem;
                        //planItem.Actor.Craft(weeklyActivity.Student.Self, weeklyActivity.FetishName);
                        _log.Log(Verbosity.Critical,  string.Format("{0} crafts device {1}",
                            weeklyActivity.Actor.CharacterName,
                            weeklyActivity.DeviceName));
                            break;
                    }

                }
            }

            //person spends learning attempts
            foreach (var actor in _party)
            {
                List<WeeklyActivity> characterActivities = WeeklyFilter.ByActor(plan, actor.Key);
                List<CreateTalensActivity> createTalens = WeeklyFilter.ByCreateTalens(characterActivities); //talens are single
                List<CreateItemActivity> createFetishBase = WeeklyFilter.ByCreateFetishItem(characterActivities); //fetishe Bases are single
                List<CreateFetishActivity> createFetish = WeeklyFilter.ByCreateFetish(characterActivities); //fetishes are single
                List<LearnAbility> learnAbility = WeeklyFilter.ByLearnAbility(characterActivities);
                List<LearnRiteFromGarou> learnRite = WeeklyFilter.ByLearnRiteFromGarou(characterActivities);
                List<LearnGiftFromGarou> learnGift = WeeklyFilter.ByLearnGiftFromGarou(characterActivities);

                foreach (var planItem in createFetishBase)
                {
                    CreateItemActivity weeklyActivity = (CreateItemActivity)planItem;
                    planItem.Actor.CreateFetishBase(weeklyActivity.Level, weeklyActivity.FetishName);
                    break;
                }

                foreach (var planItem in createFetish)
                {
                    CreateFetishActivity weeklyActivity = (CreateFetishActivity)planItem;
                    planItem.Actor.CreateFetish(weeklyActivity.Level, weeklyActivity.FetishName, weeklyActivity.SpiritType);
                }

                foreach (var planItem in createTalens)
                {
                    _log.Log(Verbosity.Critical,  "");
                    _log.Log(Verbosity.Critical,  string.Format("{0} creating talens {1}",
                        planItem.Actor.CharacterName,
                        planItem.TalenName));
                }

                //HERE WE START TO SPEND LEARNING ATTEMPTS
                //TODO create LearningPartialActions variable

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

                if (actor.Value.WeeklyPartialActions != 0)
                {
                    _log.Log(Verbosity.Error,  string.Format(
                        "{0} has {1} unused partial actions!", actor.Key, actor.Value.WeeklyPartialActions));
                }
            }

            //Show Learning Results
            var progress = GetProgress(WeekStartingTraits);

            //calculate progress for rites and items for each character
            foreach (var progressSummaryKvp in progress)
            {
                foreach (var riteProgress in progressSummaryKvp.Value.RiteProgress)
                {
                    if (riteProgress.Value.Item2 == Build.RiteAlreadyLearned)
                    {
                        var rite = RiteInfo.ByName(riteProgress.Key);
                        //learned rite this week
                        _party[progressSummaryKvp.Key].Self.Rites[rite.Rite] = weekNo;
                    }
                }
            }

            _log.Log(Verbosity.Critical,  "");
            _log.Log(Verbosity.Critical,  string.Format("=>> Week {0} results:", weekNo));
            LogProgress(progress);



            _log.Log(Verbosity.Critical,  string.Format("<==== Week {0} ends", weekNo));
        }

        private void CheckFullAndCalcPartialActions(int weekNo, List<WeeklyActivity> plan)
        {
            CheckFullAndCalcPartialActions(weekNo, plan, weekNo < 10);
        }

        private void CheckFullAndCalcPartialActions(int weekNo, List<WeeklyActivity> plan, bool bSerialArc6)
        {

            //1) Learning is extended activity; while learning you cannot do anything else! But it is partial action (due to cacao)
            //2) LearnRiteFromGarou / TeachRiteToGarou correspondence
            //3) Teaching is single activity; but TeachRiteToGarou spend partial action and must be synced
            //4) CreateDeviceActivity is an extended action; while Fetish/Talen/Base are single

            foreach (var hatysPartyMemberKvp in _party)
            {
                var initialWeeklyActions = hatysPartyMemberKvp.Value.WeeklyActions;
                List<WeeklyActivity> characterActivities = WeeklyFilter.ByActor(plan, hatysPartyMemberKvp.Key);

                var teachingActivities = WeeklyFilter.ByTeaching(characterActivities);                  //all single
                var teachRiteToGarou = WeeklyFilter.ByTeachRiteToGarou(characterActivities);            //except TeachRiteToGarou which is partial

                //var learningActivitires = WeeklyFilter.ByLearning(characterActivities);               //always extended and partial

                var creationActivities = WeeklyFilter.ByCreation(characterActivities);
                var creationExtended = WeeklyFilter.ByType(creationActivities, ActivityType.Extended);  //creation full extended (CreateDeviceActivity)
                var creationSingle = WeeklyFilter.ByType(creationActivities, ActivityType.Single);      //creation single, -1 partial (CreateTalensActivity, CreateFetish, CreateItemActivity)

                var creationTalens = WeeklyFilter.ByCreateTalens(characterActivities);
                var creationFetish = WeeklyFilter.ByCreateFetish(characterActivities);
                var creationFetishBase = WeeklyFilter.ByCreateFetishItem(characterActivities);

                //check creation single actions!
                if (creationTalens.Count + creationFetish.Count > 1)
                {
                    throw new Exception(string.Format("Only one fetish/talen per week! ({0})", hatysPartyMemberKvp.Key));
                }
                if (creationFetishBase.Count > 1)
                {
                    throw new Exception(string.Format("Only one fetish base art per week!? ({0})", hatysPartyMemberKvp.Key));
                }

                //check single actions!
                if (teachingActivities.Count > 1)
                {
                    throw new Exception(string.Format("Multiple teaching is not supported! ({0})", hatysPartyMemberKvp.Key));
                }
                if (teachRiteToGarou.Count == 0)
                {
                    hatysPartyMemberKvp.Value.WeeklyActions -= teachingActivities.Count; //teaching is a full single action
                }
                else
                {
                    string teacher = hatysPartyMemberKvp.Key;
                    string student = teachRiteToGarou[0].Student.Self.Name;
                    bool match = false;

                    List<WeeklyActivity> studentActivities = WeeklyFilter.ByActor(plan, student);
                    List<LearnRiteFromGarou> learnActivities = WeeklyFilter.ByLearnRiteFromGarou(studentActivities);
                    if (learnActivities.Count != 0)
                    {
                        foreach (var activity in learnActivities)
                        {
                            if (activity.Teacher.Self.Name == teacher)
                            {
                                if (activity.MaxLearnAttempts != teachRiteToGarou[0].MaxLearnAttempts)
                                {
                                    throw new Exception(string.Format("Learning attempts mismatch {0}/{1}", teacher, student));
                                }
                                match = true;
                            }
                        }
                    }
                    if (!match)
                    {
                        throw new Exception(string.Format("Where is the student to learn rite from {0}", teacher));
                    }
                }

                if (creationExtended.Count > 1)
                {
                    throw new Exception(string.Format("No way to perform more than one extended creation roll! ({0})", hatysPartyMemberKvp.Key));
                }

                //SIC #1
                int spentWeeklyActions = initialWeeklyActions - hatysPartyMemberKvp.Value.WeeklyActions;

                //SIC #2 spiridon casts mind partition to his extended actions.
                int extraExtendedActions = 0;
                if (hatysPartyMemberKvp.Key.Equals(Spiridon.CharacterName))
                {
                    extraExtendedActions = Spiridon.CastMindPartition();
                }

                //SIC #2
                hatysPartyMemberKvp.Value.WeeklyActions -= creationExtended.Count; //this is a FULL extended action

                if (hatysPartyMemberKvp.Value.WeeklyActions < 0)
                {
                    throw new Exception(string.Format("Not enough weekly actions! ({0})", hatysPartyMemberKvp.Key));
                }

                //SIC #3 - weekly to partial (all learning, teach rite to garou and single creations)
                if (hatysPartyMemberKvp.Key.Equals(Yoki.CharacterName))
                {
                    //yoki has x4 multiplier from Eidetic memory
                    hatysPartyMemberKvp.Value.WeeklyPartialActions = hatysPartyMemberKvp.Value.WeeklyActions * (bSerialArc6 ? 4 : 2);
                }
                else
                {
                    //all others use cacao
                    hatysPartyMemberKvp.Value.WeeklyPartialActions = hatysPartyMemberKvp.Value.WeeklyActions * 2;
                }
                int initialPartialActions = hatysPartyMemberKvp.Value.WeeklyPartialActions;

                if (teachRiteToGarou.Count != 0)
                {
                    hatysPartyMemberKvp.Value.WeeklyPartialActions -= teachRiteToGarou[0].MaxLearnAttempts; //this is a partial action
                }

                if (creationSingle.Count != 0 && creationExtended.Count != 0)
                {
                    throw new Exception(string.Format("No way to combine extended and single creation rolls! ({0})", hatysPartyMemberKvp.Key));
                }

                hatysPartyMemberKvp.Value.WeeklyPartialActions -= creationSingle.Count;                        //this is a partial action
                //all left partial actions are for LEARNING!

                if (hatysPartyMemberKvp.Value.WeeklyPartialActions < 0)
                {
                    throw new Exception(string.Format("Not enough partial weekly actions! ({0})", hatysPartyMemberKvp.Key));
                }

                _log.Log(Verbosity.Critical, string.Format("{0} has {1} full (spent {2}) and {3} partial (spent {4}) actions. MP bonus = {5}.",
                    hatysPartyMemberKvp.Key,
                    initialWeeklyActions,
                    spentWeeklyActions,
                    initialPartialActions,
                    initialPartialActions - hatysPartyMemberKvp.Value.WeeklyPartialActions,
                    extraExtendedActions));
            }

        }
    }
}

