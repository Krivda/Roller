using System.Collections.Generic;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    public class SkillRoll : RollBase
    {
        public SkillRoll(string name, IRollLogger log, RollAnalyzer roller, List<string> dicePool, List<string> conditions, string additionalInfo, Verbosity verbosity) :
            base(name, log, roller, dicePool, true, true, conditions, additionalInfo, verbosity)
        {
        }
    }
}