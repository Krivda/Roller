using System.Collections.Generic;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class GiftRoll : RollBase
    {
        public GiftRoll(string name, IRollLogger log, IRoller roller, List<string> dicePool, List<string> conditions, string addtionalInfo, Verbosity verbosity) 
            : base(name, log, roller, dicePool, true, true, conditions, addtionalInfo, verbosity)
        {
        }
    }
}