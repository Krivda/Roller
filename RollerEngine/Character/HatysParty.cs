using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Rolls.Rites;

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

            Kinfolk1 = new Kinfolk1(party["Kinfolk 1"], log, roller, this);
            _party.Add(Kinfolk1.CharacterName, Kinfolk1);

            Kinfolk2 = new Kinfolk2(party["Kinfolk 2"], log, roller, this);
            _party.Add(Kinfolk2.CharacterName, Kinfolk2);

            Lynn = new Lynn(party["Lynn"], log, roller, this);
            _party.Add(Lynn.CharacterName, Kinfolk2);

            OriginalStats = new Dictionary<string, Dictionary<string, int>>();


            Spiridon.HasOpenedCaern = true;
            Yoki.HasSpecOnInstruction = true;
            Kinfolk1.HasSpecOnInstruction = true;
            Kinfolk2.HasSpecOnInstruction = true;

            Nameless.Self.HasAncestorVeneration = true;
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller roller)
        {
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddPermanentModifiers(party, log);
            return new HatysParty(party, log, roller);
        }

        private void WeeklyBuff()
        {
            //todo: rewrite

            //boost
            Nameless.WeeklyPreBoost();
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
            _log.Log(Verbosity.Critical, "==> week summary:");

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

                            _log.Log(Verbosity.Critical, string.Format("{0} advanced in learning trait {1}: he already learned {2}XP towards next rank!. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.ExpiriencePool))
                        {
                            string baseTrait = Build.DynamicTraits.GetBaseTrait(activeTrait.Key, Build.DynamicTraits.ExpiriencePool);

                            _log.Log(Verbosity.Critical, string.Format("{0} advanced in learning trait {1}: he has {2}XP in trait pool. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.RiteLearned))
                        {
                            string baseTrait = Build.DynamicTraits.GetBaseTrait(activeTrait.Key, Build.DynamicTraits.RiteLearned);

                            if (currTraitValue == Build.RiteAlreadyLearned && currTraitValue != origTraitValue)
                            {
                                //finished learning rite this week
                                _log.Log(Verbosity.Critical, string.Format("{0} has learned rite {1}!", origKvp.Key, baseTrait));
                            }
                            else if (currTraitValue != Build.RiteAlreadyLearned)
                            {
                                _log.Log(Verbosity.Critical, string.Format("{0} advanced in learning rite {1}: he already learned {2} successes!. (changed from {3})!", origKvp.Key, baseTrait, currTraitValue, origTraitValue));
                            }
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.RitePool))
                        {
                            //skip it
                        }
                        else 
                        {
                            _log.Log(Verbosity.Critical, string.Format("{0} advanced in learning trait {1}: he increased trait value from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
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

            Spiridon.LearnAttempts = 2;
            Nameless.LearnAttempts = 2;
            Kurt.LearnAttempts = 2;
            Yoki.LearnAttempts = 4;
            Kinfolk1.LearnAttempts = 2;
            Kinfolk2.LearnAttempts = 2;
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
                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") || buildKvp.Key.Equals("Lynn"))
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

                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") || buildKvp.Key.Equals("Alisa") || buildKvp.Key.Equals("Urfin") || buildKvp.Key.Equals("Lynn"))
                {
                    buildKvp.Value.CharacterClass = Build.Classes.Werewolf;

                    //Hatys
                    if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") ||
                        buildKvp.Key.Equals("Alisa") || buildKvp.Key.Equals("Urfin"))
                    {
                        CommonBuffs.ApplyHatysBuff(buildKvp.Value, log);
                    }
                }

                if (buildKvp.Key.Equals("Kinfolk 1") || buildKvp.Key.Equals("Kinfolk 2"))
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

            _log.Log(Verbosity.Critical, string.Format("<==== Week {0} starts", weekNo));

            StartScene();

            WeeklyBuff();

            Spiridon.CastMindPartition();

            //first of all queue rites and roll teaching
            foreach (var planItem in plan)
            {
                switch (planItem.Activity)
                {
                    case WeeklyActivity.ActivityKind.Teaching:
                        planItem.Actor.Instruct(planItem.Student.Self, planItem.Trait, false);
                        break;

                    case WeeklyActivity.ActivityKind.QueueNewRite:

                        string keyRitePool = Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, planItem.RiteInfo.Name);
                        string keyRiteLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned, planItem.RiteInfo.Name);

                        //create dynamic trait if it was absent
                        if (!planItem.Actor.Self.Traits.ContainsKey(keyRitePool))
                        {
                            planItem.Actor.Self.Traits.Add(keyRitePool, (int)planItem.RiteInfo.Level * 10);
                        }

                        //create dynamic trait if it was absent
                        if (!planItem.Actor.Self.Traits.ContainsKey(keyRiteLearned))
                        {
                            planItem.Actor.Self.Traits.Add(keyRiteLearned, 0);
                        }                        
                        break;
                }
            }

            //person spends learning attempts
            foreach (var actor in _party)
            {
                List<WeeklyActivity> personalPlanCreateTalens =
                    plan.FindAll(wa => wa.Actor.CharacterName.Equals(actor.Value.CharacterName) && wa.Activity == WeeklyActivity.ActivityKind.CreateTalens);
                List<WeeklyActivity> personalPlanTeachGarouToGarou =
                    plan.FindAll(wa => wa.Actor.CharacterName.Equals(actor.Value.CharacterName) && wa.Activity == WeeklyActivity.ActivityKind.TeachGarouToGarou);
                List<WeeklyActivity> personalPlanLearnTraits =
                    plan.FindAll(wa => wa.Actor.CharacterName.Equals(actor.Value.CharacterName) && wa.Activity == WeeklyActivity.ActivityKind.LearnTrait);
                List<WeeklyActivity> personalPlanLearnRites =
                    plan.FindAll(wa => wa.Actor.CharacterName.Equals(actor.Value.CharacterName) && wa.Activity == WeeklyActivity.ActivityKind.LearnRite);

                foreach (var planItem in personalPlanTeachGarouToGarou)
                {
                    planItem.Actor.LearnAttempts -= planItem.MaxLearnAttempts;
                }

                foreach (var planItem in personalPlanCreateTalens)
                {
                    planItem.Actor.LearnAttempts -= planItem.MaxLearnAttempts;
                }

                foreach (var planItem in personalPlanLearnRites)
                {
                    planItem.Actor.AutoLearnRite(planItem.MaxLearnAttempts);
                }

                foreach (var planItem in personalPlanLearnTraits)
                {
                    planItem.Actor.AutoLearn(planItem.MaxLearnAttempts);
                }

                actor.Value.AutoLearn(HatysPartyMember.SPEND_ALL_ATTEMPTS);     //first try to learn more traits
                actor.Value.AutoLearnRite(HatysPartyMember.SPEND_ALL_ATTEMPTS); //second try to learn more rites

                //TODO: maybe log attempts.
            }

            ShowLearningResults();

            _log.Log(Verbosity.Critical, string.Format("<==== Week {0} ends", weekNo));
        }

        private List<WeeklyActivity> GetPlanByWeekNumber(int weekNo)
        {
            List<WeeklyActivity> plan = new List<WeeklyActivity>();

            switch (weekNo)
            {
                //8 Feb (teaching week)
                case 1:
                    //teach
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Nameless, Kinfolk1, Build.Abilities.Leadership));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Kinfolk1, Kurt, Build.Abilities.Firearms));

                    //learn
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Kurt, 2));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Kinfolk1, 2));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Yoki, 2));

                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.CreateTalens, Yoki, 2));

                    break;

                //15 Feb
                case 2:
                    //Kinfolks learn nothing!?

                    //Spiridon,Yoki: create Talens
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.CreateTalens, Spiridon, 2)); //TODO: or 0.5 week?
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.CreateTalens, Yoki, 2));
                    //Nameless teach Kurt Ancestor Veneration
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachGarouToGarou, Nameless, 1));

                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Nameless, RitesDictionary.Rites["of Opened Caern"]));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Kurt, RitesDictionary.Rites["of Ancestors Veneration"]));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Yoki, RitesDictionary.Rites["of Ancestors Veneration"])); //from Kurt later
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Spiridon, RitesDictionary.Rites["of Ancestors Veneration"])); //from Kurt later

                    //Spiridon PACK of rites!!!
                    /*
                     * +bone rythms
                     * 15b. rites to dictionary for planned
		Papa Serega: 5 - for Rite of Signpost(4); Rite of Trespassing(5); Crash Space (2); Shopping Chart (2)
		Golosa Vetrov: 5 - for Rite of Caern Building (5); Rite of Balance (3); Invitation to Ancestors (4)
		Babka Aine: 5 - for Rite of Sacred Peace(5)/Bowels of Mother(0), Asklepius (3), Comfort (2), Sin-Eating (3); of Teachers (1)
		Udjin: 4 - for Rite of Fetish (3); Rite of Deliverance (3); Nightshade (4); Deliverance (3); Sin-Eater (2)
                     */

                    break;

                //22 Feb (teaching week)

                case 3:
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnRite, Spiridon, RitesDictionary.Rites["Some rite 1"]));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Spiridon, RitesDictionary.Rites["Some rite 2"]));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.QueueNewRite, Spiridon, RitesDictionary.Rites["Rite of Something"]));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnRite, Spiridon, 5));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Nameless, Kinfolk1, Build.Abilities.Leadership));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Yoki, Ptitsa, Self.Abilities.Stealth)); //done
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Spiridon, Kurt, Self.Abilities.Rituals)); //can't teach that week
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Kinfolk1, Kurt, Build.Abilities.Firearms));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.Teaching, Kinfolk2, Nameless, Self.Abilities.Brawl)); //done


                    //learning
                    //plan.Add(new WeeklyActivity(Nameless, 2));
                    //plan.Add(new WeeklyActivity(Spiridon, 2));
                    //plan.Add(new WeeklyActivity(Kurt, 2));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Yoki, 4));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Yoki, 4));
                    //plan.Add(new WeeklyActivity(Kinfolk1, 2));
                    //plan.Add(new WeeklyActivity(Kinfolk2, 2));

                    break;
                case 4:
                    //learning
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Nameless, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Spiridon, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Kurt, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Yoki, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Yoki, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Kinfolk1, 1));
                    plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.LearnTrait, Kinfolk2, 1));

                    break;
            }

            return plan;
        }
    }
}