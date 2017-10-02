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
        

        public Dictionary<string, Dictionary<string, int>> WeekStartingTraits { get; set; }
        public Dictionary<string, Dictionary<string, int>> RunStartTraits { get; set; }

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

            WeekStartingTraits = new Dictionary<string, Dictionary<string, int>>();
            RunStartTraits = new Dictionary<string, Dictionary<string, int>>();


            Spiridon.HasOpenedCaern = true;
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

                foreach (var currTraitKvp in currCharacterTraits)
                {
                    string trait = currTraitKvp.Key;
                    Tuple<int, int> traitDiff = GetTraitDiff(trait, origCharacterTraits, currCharacterTraits);

                    //is ability?
                    if (typeof(Build.Abilities).GetFields().Any(fi => fi.Name.Equals(currTraitKvp.Key)))
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

                    _log.Log(Verbosity.Critical, ActivityChannel.Main,
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
                        _log.Log(Verbosity.Critical, ActivityChannel.Main,
                            string.Format("{0} has learned rite {1}.", characterName, riteName));
                    }
                    else
                    {
                        _log.Log(Verbosity.Critical, ActivityChannel.Main,
                            string.Format("{0} has progressed in learning rite {1}: {2}->{3} of {4}.", characterName,
                                riteName,
                                riteProgressKvp.Value.Item1, riteProgressKvp.Value.Item2, riteProgressKvp.Value.Item3));
                    }
                }

                foreach (var itemKvp in charProgressKvp.Value.ItemsProgress)
                {
                    string direction;
                    if (itemKvp.Value.Item1 < itemKvp.Value.Item2)
                    {
                        direction = "got";
                    }
                    else
                    {
                        direction = "lost";
                    }

                    _log.Log(Verbosity.Critical, ActivityChannel.Main,
                        string.Format("{0} has {1} item {2}. Item count: {3}->{4}", characterName, direction,
                            itemKvp.Key, itemKvp.Value.Item1, itemKvp.Value.Item2));
                }
            }
        }

        public void LogTotalProgress()
        {
            var progress = GetProgress(RunStartTraits);
            _log.Log(Verbosity.Warning, ActivityChannel.Main, "");
            _log.Log(Verbosity.Warning, ActivityChannel.Main, "");
            _log.Log(Verbosity.Warning, ActivityChannel.Main, "===== >>>> This run results:");
            LogProgress(progress);
            _log.Log(Verbosity.Warning, ActivityChannel.Main, "<<<< =====");
        }

        /*private void ShowLearningResults()
        {
            //TODO: summary channel
            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, "");
            _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, "==> week summary:");

            foreach (var origKvp in WeekStartingTraits)
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
        }*/

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

            WeekStartingTraits.Clear();
            foreach (var hatysPartyMemberKvp in _party)
            {
                WeekStartingTraits[hatysPartyMemberKvp.Value.CharacterName] = CopyTraitValues(hatysPartyMemberKvp.Value.Self);
                hatysPartyMemberKvp.Value.WeeklyActions = 1;
            }


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

                if (buildKvp.Key.Equals("Krivda"))
                {
                    buildKvp.Value.Rites.Add(RitesDictionary.Rites[Rite.AncestorVeneration].Name, 0);
                    buildKvp.Value.Rites.Add(RitesDictionary.Rites[Rite.BoneRhythms].Name, 0);
                }

                if (buildKvp.Key.Equals("Keltur"))
                {
                    CommonBuffs.ApplyMedicalBundle(buildKvp.Value, log);
                    buildKvp.Value.Rites.Add(RitesDictionary.Rites[Rite.OpenedCaern].Name, 0);
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

            _log.Week = weekNo;
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
                List<CreateFetishBase> createFetishBase = WeeklyFilter.ByCreateFetishBase(characterActivities); //fetishe Bases are single
                List<CreateFetishActivity> createFetish = WeeklyFilter.ByCreateFetish(characterActivities); //fetishes are single
                List<LearnAbility> learnAbility = WeeklyFilter.ByLearnAbility(characterActivities);
                List<LearnRiteFromGarou> learnRite = WeeklyFilter.ByLearnRiteFromGarou(characterActivities);
                List<LearnGiftFromGarou> learnGift = WeeklyFilter.ByLearnGiftFromGarou(characterActivities);

                //TODO: more checks about conflicting planItems
                if (createTalens.Count > 1)
                {
                    throw new Exception("no multiple talens"); //TODO: think?
                }

                if (createFetishBase.Count > 1)
                {
                    throw new Exception("no multiple fetish bases"); //TODO: think?
                }

                if (createFetish.Count > 1)
                {
                    throw new Exception("no multiple fetish"); //TODO: think?
                }

                if (createTalens.Count + createFetishBase.Count + createFetish.Count > 1)
                {
                    throw new Exception("no multiple talens"); //TODO: think?
                }

                foreach (var planItem in createFetishBase)
                {
                    planItem.Actor.WeeklyPartialActions -= 1;
                    CreateFetishBase weeklyActivity = (CreateFetishBase)planItem;
                    planItem.Actor.CreateFetishBase(weeklyActivity.Level, weeklyActivity.FetishName);
                    break;
                }

                foreach (var planItem in createFetish)
                {
                    planItem.Actor.WeeklyPartialActions -= 1;
                    CreateFetishActivity weeklyActivity = (CreateFetishActivity)planItem;
                    planItem.Actor.CreateFetish(weeklyActivity.Level, weeklyActivity.FetishName, weeklyActivity.SpiritType);
                }

                foreach (var planItem in createTalens)
                {
                    planItem.Actor.WeeklyPartialActions -= 1;
                    _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("{0} creating talens {1}",
                        planItem.Actor.CharacterName,
                        planItem.TalenName));
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

            //ShowLearningResults();
            var progress = GetProgress(WeekStartingTraits);
            _log.Log(Verbosity.Critical, ActivityChannel.Main, "");
            _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("=>> Week {0} results:", weekNo));
            LogProgress(progress);

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