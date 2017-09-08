using System.Collections.Generic;

namespace RolzOrgEnchancer
{
    internal struct RollInput
    {
        public const int Max = 99;     //let support max=99 dice pool
        public const int BaseDC = 6;

        public int DiceCount;
        public int DC;                  //you should cut it to 3..9/10
        public bool HasSpecialization;  // 2 successes  for each 10
        public bool NegateBotch;        // botch usually negated by spending Willpower
        public bool NoFailures;         //no -1 success for each 1 (ex. damage roll)

        public void Initialize(int diceCount, int dc = BaseDC, bool hasSpecialization = false, bool negateBotch = false, bool noFailures = false)
        {
            DiceCount = diceCount;
            DC = dc;
            HasSpecialization = hasSpecialization;
            NegateBotch = negateBotch;
            NoFailures = noFailures;
        }

        public void Validate()
        {
            if (DiceCount <= 0) throw new System.ArgumentOutOfRangeException();
            if (DiceCount > Max) throw new System.ArgumentOutOfRangeException();
            if ((DC < 3) || (DC > 10)) throw new System.ArgumentOutOfRangeException();
        }
    }

    internal struct RollOutput
    {
        //provide simple counting; for more complicated use cases process raw fields by yourself
        public int RawResult;
        public int RawNumberOfOnes;
        public int RawNumberOfTens;
        public List<int> RawDices;
        public int Result;      // <0 botch
                                // =0 simple failure
                                // >0 successes

        /*public RollOutput(RollOutput output)
        {
            _raw_result = output._raw_result;
            _raw_number_of_ones = output._raw_number_of_ones;
            _raw_number_of_tens = output._raw_number_of_tens;
            _raw_dices = new List<int>(output._raw_dices);
            result = 0;
        }*/

        public void CalculateResult(RollInput input)
        {
            Result = RawResult;                                        // success - failures
            if (input.HasSpecialization) Result += RawNumberOfTens;  // double successes if specialized
            if (input.NoFailures) Result += RawNumberOfOnes;         // negate failures
            if ((Result < 0) && (input.NegateBotch)) Result = 0;         // negate botch
        }
    }

    internal class DicePoolRoll
    {
        readonly RollInput input;
        RollOutput _output;

        public DicePoolRoll(RollInput _input) 
        {
            _input.Validate();
            input = _input;
        }

        public DicePoolRoll(DicePoolRoll roll, RollOutput output)
        { 
            input = roll.input; 
            this._output = output;
            this._output.CalculateResult(input);
        }

        /*public RollOutput GetRollOutput()
        {
            return new RollOutput(output);
        }*/

        public int GetRollResult()
        {
            return _output.Result;
        }
    }
}