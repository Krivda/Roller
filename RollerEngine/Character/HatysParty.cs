using System.Collections.Generic;
using RollerEngine.Character.Modifiers;
using RollerEngine.Character.Party;
using RollerEngine.Roller;
using RollerEngine.Logger;

namespace RollerEngine.Character
{
    public class HatysParty
    {
        public Nameless Nameless { get; private set; }
        public Spirdon Spirdon { get; private set; }
        public Yoki Yoki { get; private set; }
        public Kurt Kurt { get; private set; }

        public HatysParty(Dictionary<string, Build> party, IRollLogger log, IRoller roller)
        {
            Nameless = new Nameless(party["Krivda"], log, roller, this);
            Spirdon = new Spirdon(party["Keltur"], log, roller, this);
            Yoki = new Yoki(party["Alisa"], log, roller, this);
            Kurt = new Kurt(party["Urfin"], log, roller, this);
        }

        public static HatysParty LoadFromGoogle(IRollLogger log, IRoller roller)
        {
            var party = HatysPartyLoader.LoadFromGoogle(log);
            AddKnownModifiers(party);
            return new HatysParty(party, log, roller);
        }


        public void WeeklyLearning()
        {
            //boost nameless Instruction
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);

            //teach keltur some occult
            Nameless.Instruct(Spirdon.Build, Build.Abilities.Occult, false);


            //buff keltur's occult
            Nameless.CastTeachersEase(Spirdon.Build, Build.Abilities.Occult);

        }

        private static void AddKnownModifiers(Dictionary<string, Build> result)
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
                    buildKvp.Value.DCModifiers.Add(new DCModifer(
                        "Hatys",
                        new List<string>() { Build.Backgrounds.Ansestors },
                        DurationType.Permanent,
                        new List<string>(),
                        -2
                    ));
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