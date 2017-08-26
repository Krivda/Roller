using System;
using System.Collections.Generic;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls
{
    public class BackgroundRoll : RollBase
    {
        public BackgroundRoll(string name, IRollLogger log, IRoller roller, List<string> conditions) 
            : base(name, log, roller, new List<string>(){name}, true, true, conditions)
        {}

        protected override int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            if (hasSpec)
                throw  new Exception("Can't have specializations on Background rolls.");

            if (hasWill)
                throw new Exception("Can't apply will on Background rolls.");

            return base.Roll(actor, targets, false, false);
        }
    }
}