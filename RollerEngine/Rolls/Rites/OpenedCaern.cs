using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Rolls.Rites
{
    public class OpenedCaern : RiteRoll
    {
        /*
        System: To open a caern, a character engages in a resisted,
            extended success test of Wits + Rituals (difficulty 7). The number
        of successes needed equals the caern’s level.
            The character must overcome the caern spirit to prove
        herself worthy. The caern spirit uses its caern level as a dice
        pool. Its difficulty equals the character’s Gnosis, while the
            number of successes needed equals the character’s Willpower.
            The first one to garner the necessary number of successes wins.
         */
        public OpenedCaern(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.OpenedCaern, log, roller,
                null, //default
                new List<string>() { Build.Conditions.SpiritRite }, null, Verbosity.Critical)
        {
        }

        public new int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            //how to roll on somebody?
            return 0;
        }
    }

    public class Binding : RiteRoll
    {
        public Binding(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.Binding, log, roller,
            null, //default
            new List<string>(), null, Verbosity.Critical)
        {
        }
    }

    public class SpiritAwakening : RiteRoll
    {
        public SpiritAwakening(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.SpiritAwakening, log, roller,
                null, //default
                new List<string>(), null, Verbosity.Critical)
        {
        }
    }

    public class Summoning : RiteRoll
    {
        public Summoning(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.Summoning, log, roller,
                null, //default
                new List<string>(), null, Verbosity.Critical)
        {
        }
    }

    public class CrashSpace : RiteRoll
    {
        public CrashSpace(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.CrashSpace, log, roller,
                null, //default
                new List<string>(), null, Verbosity.Critical)
        {
        }
    }

    public class Knowing : RiteRoll
    {
        public Knowing(IRollLogger log, IRollAnalyzer roller) :
            base(Rite.Knowing, log, roller,
                null, //default
                new List<string>(), null, Verbosity.Critical)
        {
        }
    }

}