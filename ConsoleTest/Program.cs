using System;
using RollerEngine.Character;
using RollerEngine.Roller;
using RollerEngine.Rolls;

namespace ConsoleTest
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var res = HatysPartyLoader.LoadParty(new NLogLogger(Logger), new OfflineDiceRoller());
            res.Nameless.WeeklyBoostTeachersEase();
            Console.ReadKey();
        }
    }
}
