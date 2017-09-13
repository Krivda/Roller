using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Modifiers;

namespace RollerEngine.Character
{
    public class HatysParty
    {
        private readonly Dictionary<string, Build> _partyBuilds;
        private readonly Dictionary<string, HatysPartyMember> _party;

        private readonly IRollLogger _log;
        private readonly IRoller _roller;
        public Nameless Nameless { get; private set; }
        public Spirdon Spirdon { get; private set; }
        public Yoki Yoki { get; private set; }
        public Kurt Kurt { get; private set; }
        public Kinfolk1 Kinfolk1 { get; private set; }
        public Kinfolk2 Kinfolk2 { get; private set; }

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

            Spirdon = new Spirdon(party["Keltur"], log, roller, this);
            _party.Add(Spirdon.CharacterName, Spirdon);

            Yoki = new Yoki(party["Alisa"], log, roller, this);
            _party.Add(Yoki.CharacterName, Yoki);

            Kurt = new Kurt(party["Urfin"], log, roller, this);
            _party.Add(Kurt.CharacterName, Kurt);

            Kinfolk1 = new Kinfolk1(party["Kinfolk 1"], log, roller, this);
            _party.Add(Kinfolk1.CharacterName, Kinfolk1);

            Kinfolk2 = new Kinfolk2(party["Kinfolk 2"], log, roller, this);
            _party.Add(Kinfolk2.CharacterName, Kinfolk2);

            OriginalStats = new Dictionary<string, Dictionary<string, int>>();


            Spirdon.HasOpenedCaern = true;
            Yoki.HasSpecOnInstruction = true;
            Yoki.LearnSessions = 2;
            Kinfolk1.HasSpecOnInstruction = true;
            Kinfolk2.HasSpecOnInstruction = true;
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller roller)
        {
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddPermanentModifiers(party, log);
            return new HatysParty(party, log, roller);
        }

        public void TeachingWeek(List<TeachPlan> teachPlan)
        {
            _log.Log(Verbosity.Important, "");
            _log.Log(Verbosity.Warning, "Start TEACHING Week");

            WeeklyBuff();

            _log.Log(Verbosity.Warning, "Buffs Done, starting REAL job!");

            foreach (var teachingPlan in teachPlan)
            {
                _log.Log(Verbosity.Important, "");
                _log.Log(Verbosity.Important, string.Format("{0} startered learning {1} to {2}.", teachingPlan.Teacher.CharacterName, teachingPlan.Trait, teachingPlan.Student.CharacterName));
                teachingPlan.Teacher.Instruct(teachingPlan.Student.Build, teachingPlan.Trait, false);
            }

            AutoLearn();

            _log.Log(Verbosity.Warning, "END TEACHING Week");

            //OfflineDiceRoller.LogStats(_log);
        }

        public void LearnWeek()
        {

            _log.Log(Verbosity.Important, "");
            _log.Log(Verbosity.Warning, "Start LEARNING Week");

            WeeklyBuff();

            AutoLearn();

            //OfflineDiceRoller.LogStats(_log);
        }

        private void WeeklyBuff()
        {
            StartScene();

            Nameless.CastPersuasion();

            Spirdon.ShiftToCrinos();

            Nameless.CastTeachersEase(Spirdon.Build, Build.Abilities.Rituals, true);

            Spirdon.CastSacredFire();

            //Spiridon buffs offult to Nameless
            Spirdon.CastCallToWyld(new List<Build>() { Nameless.Build });

            //boost nameless Instruction
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);

            Nameless.CastTeachersEase(Spirdon.Build, Build.Abilities.Subterfuge, false);
            Spirdon.CastPersuasion();

            Nameless.CastTeachersEase(Yoki.Build, Build.Abilities.Subterfuge, false);
            Yoki.CastPersuasion();

            Nameless.CastTeachersEase(Kinfolk1.Build, Build.Abilities.Subterfuge, false);
            Kinfolk1.CastPersuasion();

            Nameless.CastTeachersEase(Kinfolk2.Build, Build.Abilities.Subterfuge, false);
            Kinfolk2.CastPersuasion();
        }

        private void AutoLearn()
        {
            _log.Log(Verbosity.Important, "");
            _log.Log(Verbosity.Important, "Start LEARNING");

            foreach (var partyKvp in _party)
            {
                partyKvp.Value.AutoLearn();
            }
            ShowLearningResults();

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
                        if (activeTrait.Key.Equals(Build.Atributes.Strength) ||
                            activeTrait.Key.Equals(Build.Atributes.Dexterity) ||
                            activeTrait.Key.Equals(Build.Atributes.Stamina)
                            )
                        {
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.ExpirienceLearned))
                        {
                            _log.Log(Verbosity.Important, string.Format("{0} changed known XP for {1} trait from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
                        }
                        else if (activeTrait.Key.Contains(Build.DynamicTraits.ExpirienceToLearn))
                        {
                            _log.Log(Verbosity.Important, string.Format("{0} XP pool (to consume) for {1} trait from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
                        }
                        else 
                        {
                            _log.Log(Verbosity.Important, string.Format("{0} increased {1} trait from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
                        }
                    }
                }

                _log.Log(Verbosity.Important, "");
            }
        }

        private void StartScene()
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
            }

            OriginalStats.Clear();
            StoreOriginalValues(Nameless.Build);
            StoreOriginalValues(Spirdon.Build);
            StoreOriginalValues(Yoki.Build);
            StoreOriginalValues(Kurt.Build);
            StoreOriginalValues(Kinfolk1.Build);
            StoreOriginalValues(Kinfolk2.Build);
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

                //Ansestors
                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ansestors] = 5;
                }

                if (buildKvp.Key.Equals("Alisa"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ansestors] = 2;
                }

                if (buildKvp.Key.Equals("Urfin"))
                {
                    buildKvp.Value.Traits[Build.Backgrounds.Ansestors] = 1;
                }

                if (buildKvp.Key.Equals("Keltur"))
                {
                    CommonBuffs.ApplyMedicalBundle(buildKvp.Value, log);
                }

                //Hatys
                if (buildKvp.Key.Equals("Krivda") || buildKvp.Key.Equals("Keltur") || buildKvp.Key.Equals("Alisa") || buildKvp.Key.Equals("Urfin"))
                {
                    buildKvp.Value.CharacterClass = Build.Classes.Warewolf;

                    buildKvp.Value.DCModifiers.Add(new DCModifer(
                        "Hatys",
                        new List<string>() { Build.Backgrounds.Ansestors },
                        DurationType.Permanent,
                        new List<string>(),
                        -2
                    ));
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
    }
}