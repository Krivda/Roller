using System;
using NLog;
using RollerEngine.Character;
using RollerEngine.Roller;
using RollerEngine.Rolls;

namespace ConsoleTest
{
    class Program
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {

            var res = HatysPartyLoader.LoadParty(new NLogLogger(logger), new OfflineDiceRoller());

            res.Nameless.WeeklyBoostTeachersEase();
            Console.ReadKey();
        }
    }
}
