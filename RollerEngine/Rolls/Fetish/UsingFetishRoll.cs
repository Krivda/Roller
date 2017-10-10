using System;
using System.Collections.Generic;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Fetish
{
    public class UsingFetishRoll : RollBase
    {
        public UsingFetishRoll(string name, IRollLogger log, IRollAnalyzer roller, List<string> dicePool, List<string> conditions, string additionalInfo, Verbosity verbosity) :
            base(name, log, roller, dicePool, true, true, conditions, additionalInfo, verbosity)
        {
        }
    }
}