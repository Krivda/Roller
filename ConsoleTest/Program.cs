using System;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace ConsoleTest
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var logRoller = new NLogLogger(Logger);

            var res = HatysPartyLoader.LoadParty(logRoller, new OfflineDiceRoller(logRoller));
            res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
            Console.ReadKey();
        }
    }
}
