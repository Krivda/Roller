using System;
using System.Collections.Generic;
using RolzOrgEnchancer.RoomLog;

namespace RolzOrgEnchancer
{
    //with specialization every 10 counts as 2 successes
    internal enum Specialization
    {
        False,
        UsingSpecialization
    }

    //using Willpower negates botch
    internal enum NegateBotch
    {
        False,
        UsingWillpower
    }

    //(ex. damage rolls) every 1 do not count as failure
    internal enum IgnoreFailures
    {
        False,
        IgnoreFailures
    }

    //may throw exceptions
    internal struct RollInput
    {
        public const int Max = 99; //let support max=99 dice pool
        public const int BaseDc = 6;

        public readonly int DiceCount;
        public readonly int Dc; // you should cut it to 3..9/10
        public readonly Specialization HasSpecialization; // 2 successes  for each 10
        public readonly NegateBotch NegateBotch; // botch usually negated by spending Willpower
        public readonly IgnoreFailures IgnoreFailures; // no -1 success for each 1 (ex. damage roll)

        public RollInput(int diceCount, int dc = BaseDc,
            Specialization hasSpecialization = Specialization.False,
            NegateBotch negateBotch = NegateBotch.False,
            IgnoreFailures noFailures = IgnoreFailures.False)
        {
            DiceCount = diceCount;
            Dc = dc;
            HasSpecialization = hasSpecialization;
            NegateBotch = negateBotch;
            IgnoreFailures = noFailures;
            Validate();
        }

        private void Validate()
        {
            if (DiceCount <= 0) throw new ArgumentOutOfRangeException();
            if (DiceCount > Max) throw new ArgumentOutOfRangeException();
            if (Dc < 3 || Dc > 10) throw new ArgumentOutOfRangeException();
        }
    }

    //may throw exceptions
    internal struct RollOutput
    {
        public int Result;
        public List<int> RawDices;
        public int RawSuccesses;
        public int RawFailures;
        public int RawResult;
        public int RawNumberOfTens;

        //provides simple counting; for more complicated use cases process raw fields by yourself
        public RollOutput(Item item, RollInput input)
        {
            RawSuccesses = 0;
            RawFailures = 0;
            RawNumberOfTens = 0;

            if (!item.details.StartsWith("( (")) throw new Exception("Invalid details 1");
            var index = item.details.IndexOf('→', 0);
            if (index == -1) throw new Exception("Invalid details 2");
            var res = item.details.Substring(0, index);
            res = res.Substring(3);
            var res2 = res.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (res2.Length != input.DiceCount) throw new Exception("Invalid dice count");

            RawDices = new List<int>();
            foreach (var r in res2)
            {
                int i;
                if (!int.TryParse(r, out i)) throw new Exception("Not a number dice value");
                if (i < 1 || i > 10) throw new Exception("Invalid dice value");
                RawDices.Add(i);
                if (i == 10) RawNumberOfTens++;
                if (i == 1) RawFailures++;
                if (i >= input.Dc) RawSuccesses++;
            }

            RawResult = RawSuccesses - RawFailures; // success - failures

            var res3 = Convert.ToInt16(item.result); //May throw exception
            if (res3 != RawResult) throw new Exception("Raw result mismatch");

            Result = RawResult;
            if (input.HasSpecialization == Specialization.UsingSpecialization)
                Result += RawNumberOfTens; // double successes if specialized
            if (input.IgnoreFailures == IgnoreFailures.IgnoreFailures) Result += RawFailures; // negate failures
            if (Result < 0 && input.NegateBotch == NegateBotch.UsingWillpower) Result = 0; // negate botch
        }

    }
}
