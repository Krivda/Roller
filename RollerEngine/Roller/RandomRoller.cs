using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RollerEngine.Roller
{
    public class RandomRoller : IRoller
    {
        private static class RNG
        {
            private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

            public static int Next(int minValue, int maxValue)
            {
                var uint32Buffer = new byte[4];
                if (minValue > maxValue) throw new ArgumentOutOfRangeException("minValue");
                if (minValue == maxValue) return minValue;
                Int64 diff = maxValue - minValue;
                while (true)
                {
                    Rng.GetBytes(uint32Buffer);
                    uint rand = BitConverter.ToUInt32(uint32Buffer, 0);

                    Int64 max = (1 + (Int64)UInt32.MaxValue);
                    Int64 remainder = max % diff;
                    if (rand < max - remainder)
                    {
                        return (int)(minValue + (rand % diff));
                    }
                }
            }
        }

        private static int _diceFace = 10;
 
        public static void InitDiceFace(int diceFace)
        {
            _diceFace = diceFace;
        }

        public List<int> Roll(int diceCount, int DC)
        {
            var rawResult = new List<int>(_diceFace);
            rawResult.AddRange(new int[10]);

            for (var i = 0; i < diceCount; i++)
            {
                var resultOnDiceRoll = RNG.Next(1, _diceFace + 1);
                rawResult[resultOnDiceRoll - 1]++;
            }

            return rawResult;
        }
    }
}