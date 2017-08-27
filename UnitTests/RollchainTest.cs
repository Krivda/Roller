using System;
using System.IO;
using NLog;
using NUnit.Framework;
using RollerEngine.Character;
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
            var res = HatysPartyLoader.LoadParty(rollLogger, new OfflineDiceRoller(rollLogger));

            res.WeeklyLearning();
        }
    }
}