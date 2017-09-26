using RollerEngine.Character.Common;

namespace RollerEngine.Character.Party
{
    public class NamelessBuff
    {
        public class ApplyBuffs
        {
            public string Trait { get; set; }
            public bool UseRageChanneling{ get; set; }
            public bool UseBoneRhythms { get; set; }

            public ApplyBuffs(string trait, bool useRageChanneling, bool useBoneRhythms)
            {
                Trait = trait;
                UseRageChanneling = useRageChanneling;
                UseBoneRhythms = useBoneRhythms;
            }
        }

        public ApplyBuffs PreBuff { get; set; }    //weekly pre buff
        public ApplyBuffs MainBuff { get; set; }   //weekly main buff with full boost Instruct
        public ApplyBuffs LearnBuff { get; set; }  //first partial action for learning

        public const string AutoDetect = "AUTODETECT";

        public static NamelessBuff MaxBoostInstruct()
        {
            return new NamelessBuff()
            {
                PreBuff = new ApplyBuffs(Build.Abilities.Occult, false, false),
                MainBuff = new ApplyBuffs(Build.Abilities.Instruction, true, true),
                LearnBuff = new ApplyBuffs(null, false, true)
            };
        }

        public static NamelessBuff BoostSecondaryTrait(string trait)
        {
            return new NamelessBuff()
            {
                PreBuff = new ApplyBuffs(null, false, false),
                MainBuff = new ApplyBuffs(Build.Abilities.Instruction, true, true),
                LearnBuff = new ApplyBuffs(AutoDetect, false, true)
            };
        }

        public static NamelessBuff MaxBoostSecondaryTrait(string trait)
        {
            return new NamelessBuff()
            {
                PreBuff = new ApplyBuffs(null, false, false),
                MainBuff = new ApplyBuffs(Build.Abilities.Instruction, false, false),
                LearnBuff = new ApplyBuffs(AutoDetect, true, true)
            };
        }
    }
}