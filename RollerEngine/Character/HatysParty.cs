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
        private readonly Dictionary<string, Build> _party;
        private readonly IRollLogger _log;
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
            _party = party;
            _log = log;
            Nameless = new Nameless(party["Krivda"], log, roller, this);
            Spirdon = new Spirdon(party["Keltur"], log, roller, this);
            Yoki = new Yoki(party["Alisa"], log, roller, this);
            Kurt = new Kurt(party["Urfin"], log, roller, this);

            Kinfolk1 = new Kinfolk1(party["Kinfolk 1"], log, roller, this);
            Kinfolk2 = new Kinfolk2(party["Kinfolk 2"], log, roller, this);

            OriginalStats = new Dictionary<string, Dictionary<string, int>>();


            Spirdon.HasOpenedCaern = true;
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller roller)
        {
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddPermanentModifiers(party);
            return new HatysParty(party, log, roller);
        }

        public void WeeklyTeaching(List<TeachPlan> teachPlan)
        {
            StartScene();

            //boost nameless Instruction
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);

            Spirdon.CastPersuasion();

            foreach (var teachingPlan in teachPlan)
            {
                _log.Log(Verbosity.Important, "");
                _log.Log(Verbosity.Important, string.Format("{0} startered learning {1} to {2}.", teachingPlan.Teacher.CharacterName, teachingPlan.Trait, teachingPlan.Student.CharacterName));
                teachingPlan.Teacher.Instruct(teachingPlan.Student.Build, teachingPlan.Trait, false);
            }

            ShowLearningResults();
        }

        public void WeeklyLearning()
        {
            StartScene();
            
            //boost nameless Instruction
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);

            //teach keltur some occult
            Nameless.Instruct(Spirdon.Build, Build.Abilities.Occult, false);

            //buff keltur's occult
            Nameless.CastTeachersEase(Spirdon.Build, Build.Abilities.Occult);
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
                var activeTraits = _party.First(p => p.Value.Name.Equals(origKvp.Key)).Value.Traits;

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
                        _log.Log(Verbosity.Important, "");
                        _log.Log(Verbosity.Important, string.Format("{0} learned {1} from {2} to {3}!", origKvp.Key, activeTrait.Key, origTraitValue, currTraitValue));
                    }
                }
            }
        }

        private void StartScene()
        {
            CaernChannellingUsedTimes = 0;

            foreach (var build in _party)
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

        private static void AddPermanentModifiers(Dictionary<string, Build> result)
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