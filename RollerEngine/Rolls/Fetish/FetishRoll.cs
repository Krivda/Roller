using System;
using System.Collections.Generic;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Fetish
{
    public class FetishRoll : RollBase
    {
        public FetishRoll(string name, IRollLogger log, IRoller roller, List<string> dicePool, List<string> conditions, string additionalInfo, Verbosity verbosity) :
            base(name, log, roller, dicePool, true, true, conditions, additionalInfo, verbosity)
        {
        }
    }
}