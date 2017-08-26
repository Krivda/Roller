using NLog;
using NUnit.Framework;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls;
using Logger = NUnit.Framework.Internal.Logger;

namespace UnitTests
{
    class RollchainTest
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestNameLessBuff()
        {


            var res = HatysPartyLoader.LoadParty(new NLogLogger(logger), new OfflineDiceRoller());

            res.Nameless.WeeklyBoostTeachersEase();
        }
    }
}