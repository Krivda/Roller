using System.Collections.Generic;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Rites
{
    public class RiteRoll : RollBase
    {
        public RiteRoll(string name, IRollLogger log, IRoller roller, List<string> dicePool, List<string> conditions) : base(name, log, roller, dicePool, true, true, conditions)
        {
        }
    }
}