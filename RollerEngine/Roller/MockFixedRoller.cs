using System;
using System.Collections.Generic;
using RollerEngine.Logger;

namespace RollerEngine.Roller
{
    public class MockFixedRoller : IRoller
    {
        private IRollLogger _rollLogger;
        private readonly List<int> _rollResults;
        private int _rollIndex;
        public int Successes { get; set; }

        public MockFixedRoller(IRollLogger rollLogger)
        {
            _rollLogger = rollLogger;
        }

        public MockFixedRoller(IRollLogger rollLogger, List<int> rollResults)
        {
            _rollLogger = rollLogger;
            _rollResults = rollResults;
            _rollIndex = 0;

            NextResult();
        }

        private void NextResult()
        {

            if (_rollResults != null && _rollIndex < _rollResults.Count)
            {
                Successes = _rollResults[_rollIndex];
            }
        }

        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill,
            string description)
        {
            _rollLogger.Log(Verbosity.Important, string.Format("Got {0} successes on mocked fixed roll.", Successes));

            int result = Successes;
            _rollIndex++;
            NextResult();

            return new RollData(result, new List<int>());
        }
    }
}