using System;
using System.Collections.Generic;
using RollerEngine.Logger;

namespace RollerEngine.Roller
{
    public class MockFixedRoller : IRoller
    {
        private IRollLogger _rollLogger;
        public int Successes { get; set; }

        public MockFixedRoller(IRollLogger rollLogger)
        {
            _rollLogger = rollLogger;
        }

        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill,
            string description)
        {
            _rollLogger.Log(Verbosity.Important, string.Format("Got {0} successes on mocked fixed roll.", Successes));

            return new RollData(Successes, new List<int>());
        }
    }
}