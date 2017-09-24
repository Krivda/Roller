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
    public class HatysParty
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

        public void WeeklyBuff()
        {
            //todo: rewrite

            //boost
            Spiridon.ActivateCarnyx();
            Nameless.WeeklyPreBoost(Build.Abilities.Occult);
            Spiridon.DeactivateCarnyx();

            Spiridon.WeeklyPreBoost(Build.Abilities.Empathy); //it is important to Spiridon to be second due to -1 dc of Teacher's Ease of Nameless
            Spiridon.WeeklyBoostSkill(Build.Abilities.Rituals);
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
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


            //todo: make if for week #1 (hardcoded) actions 2; partion actions -= 1 for teach (NOW)
            Spiridon.WeeklyActions = 1;
            Nameless.WeeklyActions = 1;
            Kurt.WeeklyActions = 1;
            Yoki.WeeklyActions = 1;
            Kinfolk1.WeeklyActions = 1;
            Kinfolk2.WeeklyActions = 1;
            Lynn.WeeklyActions = 1;
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
            List<WeeklyActivity> plan = GetPlanByWeekNumber(weekNo);

            _log.Log(Verbosity.Critical, ActivityChannel.Main, string.Format("<==== Week {0} starts", weekNo));

            StartScene();

            CheckFullAndCalcPartialActions(weekNo, plan); //CastMindPartition is inside

            WeeklyBuff();

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

                _log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} has {1} full and {2} partial actions.",
                    hatysPartyMemberKvp.Key,
                    initialWeeklyActions,
                    hatysPartyMemberKvp.Value.WeeklyPartialActions));

            }

        }

        private List<WeeklyActivity> GetPlanByWeekNumber(int weekNo)
        {
            List<WeeklyActivity> plan = new List<WeeklyActivity>();

            switch (weekNo)
            {
                //8 Feb (teaching week)
                case 1:
                    //teach
                    plan.Add(new TeachAbility(Nameless, Kinfolk1, Build.Abilities.Leadership));
                    plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new TeachAbility(Kinfolk1, Kurt, Build.Abilities.Firearms));

                    //learn
                    plan.Add(new LearnAbility(Kurt, 2));
                    plan.Add(new LearnAbility(Kinfolk1, 2));
                    plan.Add(new LearnAbility(Yoki, 2));

                    plan.Add(new CreateTalens(Yoki, "Cacao", 1));
                    break;

                //15 Feb (no teaching week)
                case 2:
                    //Kinfolks learn nothing special

                    //Spiridon,Yoki: create Talens
                    plan.Add(new CreateTalens(Spiridon, "Cacao", 1));
                    plan.Add(new CreateTalens(Yoki, "Cacao", 1));

                    //Nameless teach Kurt Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Nameless, Kurt, Rite.AncestorVeneration, 1));
                    //Spirit teach Yoki Ancestor Seeking
                    plan.Add(new TeachRiteToGarou(Spiridon, Yoki, Rite.AncestorSeeking, 1));

                    plan.Add(new QueueRiteLearning(Kurt, Rite.AncestorVeneration));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.AncestorSeeking));

                    //MAIN common RITES for nearest future (teached from summoned spirits)
                    plan.Add(new QueueRiteLearning(Nameless, Rite.OpenedCaern));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueRiteLearning, Spiridon, Rite.OpenedCaern)); - already learned
                    plan.Add(new QueueRiteLearning(Kurt, Rite.OpenedCaern));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.OpenedCaern));

                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueRiteLearning, Nameless, Rite.BoneRhythms)); -already learned
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.BoneRhythms));
                    plan.Add(new QueueRiteLearning(Kurt, Rite.BoneRhythms));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.BoneRhythms));

                    plan.Add(new QueueRiteLearning(Yoki, Rite.SpiritAwakening));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.RenewingTalen));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.RenewingTalen));

                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Fetish));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.InvitationToAncestors));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.FeastForSpirits));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Teachers));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.SacredPeace));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.CaernBuilding));
                    break;

                //22 Feb (teaching week)
                case 3:
                    break;

                    //TODO
                    //Lynn: Talisman Dedication, Cleanising
                    //Ancestor Seeking: Krivda; Keltur; <Yoki> ...
                    //Ancestor Veneration: Krivda; <Kurt> ...

                    //Spiridon PACK of rites!!!
                    /*
                     * +bone rythms
                     * 15b. rites to dictionary for planned
		Papa Serega: 5 - for Rite of Signpost(4); Rite of Trespassing(5); Crash Space (2); Shopping Chart (2)
		Golosa Vetrov: 5 - for Rite of Caern Building (5); Rite of Balance (3); Invitation to Ancestors (4)
		Babka Aine: 5 - for Rite of Sacred Peace(5)/Bowels of Mother(0), Asklepius (3), Comfort (2), Sin-Eating (3); of Teachers (1)
		Udjin: 4 - for Rite of FetishRoll (3); Rite of Deliverance (3); Nightshade (4); Deliverance (3); Sin-Eater (2)
                     */
 

                

                case 4:
                    plan.Add(new LearnRite(Spiridon, 1));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.AncestorSeeking));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.AncestorSeeking));
                    plan.Add(new LearnRite(Spiridon, 5));
                    plan.Add(new TeachAbility(Nameless, Kinfolk1, Build.Abilities.Leadership));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Yoki, Ptitsa, Self.Abilities.Stealth)); //done
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Spiridon, Kurt, Self.Abilities.Rituals)); //can't teach that week
                    plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new TeachAbility(Kinfolk1, Kurt, Build.Abilities.Firearms));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Kinfolk2, Nameless, Self.Abilities.Brawl)); //done


                    //learning
                    //plan.Add(new WeeklyActivity(Nameless, 2));
                    //plan.Add(new WeeklyActivity(Spiridon, 2));
                    //plan.Add(new WeeklyActivity(Kurt, 2));
                    plan.Add(new LearnAbility(Yoki, 4));
                    plan.Add(new LearnAbility(Yoki, 4));
                    //plan.Add(new WeeklyActivity(Kinfolk1, 2));
                    //plan.Add(new WeeklyActivity(Kinfolk2, 2));

                    break;
                case 5:
                    //learning
                    plan.Add(new LearnAbility(Nameless, 1));
                    plan.Add(new LearnAbility(Spiridon, 1));
                    plan.Add(new LearnAbility(Kurt, 1));
                    plan.Add(new LearnAbility(Yoki, 1));
                    plan.Add(new LearnAbility(Yoki, 1));
                    plan.Add(new LearnAbility(Kinfolk1, 1));
                    plan.Add(new LearnAbility(Kinfolk2, 1));

                    break;
            }

            return plan;
        }

    }
}