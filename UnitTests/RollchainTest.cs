using System;
using System.Collections.Generic;
using System.IO;
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

            var plan = new List<TeachPlan>
            {
                new TeachPlan(res.Nameless, res.Yoki, Build.Abilities.Brawl),
                new TeachPlan(res.Yoki, res.Kurt, Build.Abilities.Rituals),
                new TeachPlan(res.Kinfolk1, res.Kinfolk1, Build.Abilities.Science)
            };

            res.TeachingWeek(plan);
        }
    }
}