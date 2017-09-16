using System.Collections.Generic;
using NLog;
using NUnit.Framework;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace UnitTests
{
    class RollchainTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestNameLessBuff()
        {
            var rollLogger = new NLogLogger(Logger);
            Logger.Info("Started");
            var res = HatysParty.LoadFromGoogle(rollLogger, new OfflineDiceRoller(rollLogger));
        }

        [Test]
        public void TestInstructionWeek()
        {
            var rollLogger = new NLogLogger(Logger);
            Logger.Info("Started");

            var res = HatysParty.LoadFromGoogle(rollLogger, new OfflineDiceRoller(rollLogger));

            var plan = new List<WeeklyActivity>
            {
                new WeeklyActivity(res.Nameless, res.Yoki, Build.Abilities.Brawl),
                new WeeklyActivity(res.Yoki, res.Kurt, Build.Abilities.Rituals),
                new WeeklyActivity(res.Kinfolk1, res.Kinfolk2, Build.Abilities.Science)
            };

            res.TeachingWeek(plan);
        }


        [Test]
        public void TestSpiridonBuff()
        {
            var rollLogger = new NLogLogger(Logger);
            var devNullLogger = new StringBufferLogger();
            var roller = new OfflineDiceRoller(rollLogger);
            var devNullRoller = new OfflineDiceRoller(devNullLogger);

            var res = HatysParty.LoadFromGoogle(rollLogger, roller);

            Logger.Info("Started");
            res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
            Logger.Info("---nameless buff skipped");

            res.Spiridon.ShiftToCrinos();

            res.Spiridon.Log = rollLogger;
            res.Spiridon.Roller = roller;

            //typeof(Spirdon).GetProperty("Log").SetValue(res.Spiridon, rollLogger, null);

            res.Spiridon.WeeklyBoostSkill(Build.Abilities.Instruction);
        }


        [Test]
        public void Mutiweek()
        {
            
        }

    }
}