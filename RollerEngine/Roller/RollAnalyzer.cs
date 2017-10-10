using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Logger;

namespace RollerEngine.Roller
{
    public class RollAnalyzer : IRollAnalyzer
    {
        private readonly IRoller _roller;
        private static int _diceFace = 10;
        private static List<int> _diceFaceStat = new List<int>(_diceFace);

        public static void LogStats(IRollLogger rollLogger)
        {
            if (_diceFaceStat != null)
            {
                int sum = _diceFaceStat.Sum();
                for (var index = 0; index < _diceFaceStat.Count; index++)
                {
                    rollLogger.Log(Verbosity.Important, 
                        string.Format("Percetage of {0} is {1:P2} (of {2} dices)", index + 1, decimal.Divide(_diceFaceStat[index], sum), sum));
                }
            }
        }

        /// <inheritdoc />
        static RollAnalyzer()
        {
            _diceFaceStat.AddRange(new int[10]);
        }

        public RollAnalyzer(IRoller roller)
        {
            _roller = roller;
        }

        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
        {
            var rawResults = _roller.Roll(diceCount, DC);
            var rollData = GetRollStats(rawResults, diceCount, DC, removeSuccessOnOnes, hasSpecialization, hasWill);
            return rollData;
        }

        /// <summary>
        /// Makes roll with specified diceCount
        /// </summary>
        /// <param name="rawResult">List, where List[4]= numeber of dice, which turned out 4</param>
        /// <param name="diceCount">dice count</param>
        /// <param name="DC">roll DC</param>
        /// <param name="removeSuccessOnOnes">whether to remove successes on ones</param>
        /// <param name="hasSpecialization">whether we have specialization (double successes on maxFace rolls or not)</param>
        /// <param name="hasWill">makes botched rolls simple failures (0 success)</param>
        /// <returns>number of successes</returns>
        private static RollData GetRollStats(List<int> rawResult, int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
        {
            if (DC < 2)
            {
                throw new Exception(string.Format("DC {0} is too low", DC));
            }

            int successCount = 0;
            int dices = 0;

            for (int i = 0; i < _diceFace; i++)
            {
                int multiplier = 1;
                if (removeSuccessOnOnes && i + 1 == 1)
                {
                    multiplier = -1;
                }
                else if (i +1 < DC)
                {
                    multiplier = 0;
                }
                else if (hasSpecialization && i + 1 == _diceFace)
                {
                    multiplier = 2;
                }

                successCount += rawResult[i] * multiplier;
                dices += rawResult[i];
                //add to statistics
                _diceFaceStat[i] += rawResult[i];
            }

            if (dices != diceCount)
            {
                throw new Exception("dice count mismatch");
            }

            if (successCount < 0 && hasWill)
            {
                successCount = 0;
            }

            var rollData = new RollData(successCount, rawResult);
            return rollData;
        }
    }
}

