using System.Collections.Generic;

namespace RollerEngine.Roller
{
    public class MockFixedRoller : IRollAnalyzer
    {
        private readonly List<int> _rollResults;
        private int _rollIndex;
        public int Successes { get; set; }

        public MockFixedRoller(List<int> rollResults)
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

        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
        {
            int result = Successes;
            _rollIndex++;
            NextResult();

            return new RollData(result, new List<int>());
        }
    }
}