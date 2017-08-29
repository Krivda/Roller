using System;
using System.Text;
using RollerEngine.Logger;

namespace RollerEngine.Roller
{
    public class OfflineDiceRoller : IRoller
    {
        private readonly IRollLogger _rollLogger;

        private static int _diceFace=10;

        private static readonly Random rnd = new Random();

        public OfflineDiceRoller(IRollLogger rollLogger)
        {
            _rollLogger = rollLogger;
        }

        public static void InitDiceFace(int diceFace)
        {
            _diceFace = diceFace;
        }


        public RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description)
        {
            var info = MakeRoll(diceCount, DC, removeSuccessOnOnes, hasSpecialization, hasWill);
            var rollData = new RollData();
            rollData.Successes = info.Item1;

            StringBuilder bld = new StringBuilder(100);
            String delim = "";
            int face = 1;
            foreach (var dice in info.Item2)
            {
                bld.Append(string.Format("{0}{1}:{2}", delim, face, dice));
                face++;
                delim = ", ";
                rollData.DiceResult.Add(info.Item1);
            }

            _rollLogger.Log(Verbosity.Details, string.Format("{0} roll was [{1}] and gave {2} successes.", description, bld, info.Item1));
            
            

            return rollData;

        }

        /// <summary>
        /// Makes roll with specified diceCount
        /// </summary>
        /// <param name="diceCount">dice count</param>
        /// <param name="DC">DC to achieve success</param>
        /// <param name="removeSuccessOnOnes">whether to remove successes on ones</param>
        /// <param name="hasSpecialization">whether we have specialization (double successes on maxFace rolls or not)</param>
        /// <param name="hasWill">makes botched rolls simple failures (0 success)</param>
        /// <returns>number of successes</returns>
        public static Tuple<int, int[]> MakeRoll(int diceCount, int DC, bool removeSuccessOnOnes,  bool hasSpecialization, bool hasWill)
        {
            int[] rollValues = new int[_diceFace];

            //make roll, store results to array
            for (int i = 0; i < diceCount; i++)
            {
                int resultOnDiceRoll = RollSingleDice(_diceFace);
                rollValues[resultOnDiceRoll - 1]++;
            }

            int successCount = GetRollSuccesses(rollValues, DC, removeSuccessOnOnes, hasSpecialization, hasWill);

            return new Tuple<int, int[]>(successCount, rollValues);
        }

        private static int GetRollSuccesses(int[] roll, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
        {
            if (DC < 2)
            {
                throw new Exception(string.Format("DC {0} is too low", DC));
            }

            int successCount = 0;

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

                successCount += roll[i] * multiplier;
            }

            if (successCount < 0 && hasWill)
            {
                successCount = 0;
            }

            return successCount;
        }

        private static int RollSingleDice(int maxDiceFace)
        {
            return rnd.Next(1, maxDiceFace + 1);
        }

    }
}