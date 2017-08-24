using System.Collections.Generic;
using RollerEngine.Roller;

namespace RollerEngine.Rolls
{
    public class GiftRoll : RollBase
    {
        public GiftRoll(string name, ILogger log, IRoller roller, List<string> dicePool, List<string> conditions) : base(name, log, roller, dicePool, true, true, conditions)
        {
        }
    }
}