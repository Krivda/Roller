using System;
using System.Collections.Generic;
using RollerEngine.Logger;

namespace RollerEngine.Roller
{
    public class MockFixedRoller : IRoller
    {
        private readonly List<int> _rollResults;
        private int _rollIndex;
        public int Successes { get; set; }

        //TODO FIX ME
        public MockFixedRoller(IRollLogger rollLogger, List<int> rollResults)
        {
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
            int result = Successes;
            _rollIndex++;
            NextResult();

            return new RollData(result, new List<int>());
        }

        public List<int> Roll(int diceCount, int DC)
        {
            return _rollResults;
        }
    }
}