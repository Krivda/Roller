using System;
using System.Collections.Generic;

namespace WOD
{
    struct RollInput
    {
        public const int max = 99;     //let support max=99 dice pool
        public const int baseDC = 6;

        public int diceCount;
        public int DC;                  //you should cut it to 3..9/10
        public bool hasSpecialization;  // 2 successes  for each 10
        public bool negateBotch;        // botch usually negated by spending Willpower
        public bool noFailures;         //no -1 success for each 1 (ex. damage roll)

        public void Initialize(int _diceCount, int _DC = baseDC, bool _hasSpecialization = false, bool _negateBotch = false, bool _noFailures = false)
        {
            diceCount = _diceCount;
            DC = _DC;
            hasSpecialization = _hasSpecialization;
            negateBotch = _negateBotch;
            noFailures = _noFailures;
        }

        public void Validate()
        {
            if (diceCount <= 0) throw new System.ArgumentOutOfRangeException();
            if (diceCount > max) throw new System.ArgumentOutOfRangeException();
            if ((DC < 3) || (DC > 10)) throw new System.ArgumentOutOfRangeException();
        }
    }

    partial struct RollOutput
    {
        //provide simple counting; for more complicated use cases process raw fields by yourself
        public int _raw_result;
        public int _raw_number_of_ones;
        public int _raw_number_of_tens;
        public List<int> _raw_dices;
        public int result;      // <0 botch
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
            result = _raw_result;                                        // success - failures
            if (input.hasSpecialization) result += _raw_number_of_tens;  // double successes if specialized
            if (input.noFailures) result += _raw_number_of_ones;         // negate failures
            if ((result < 0) && (input.negateBotch)) result = 0;         // negate botch
        }
    }

    class DicePoolRoll
    {
        RollInput input;
        RollOutput output;

        public DicePoolRoll(RollInput _input) 
        {
            _input.Validate();
            input = _input;
        }

        public DicePoolRoll(DicePoolRoll _roll, RollOutput _output)
        { 
            input = _roll.input; 
            output = _output;
            output.CalculateResult(input);
        }

        /*public RollOutput GetRollOutput()
        {
            return new RollOutput(output);
        }*/

        public int GetRollResult()
        {
            return output.result;
        }
    }
}