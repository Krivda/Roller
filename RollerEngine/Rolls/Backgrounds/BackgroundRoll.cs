using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Backgrounds
{
    public class BackgroundRoll : RollBase
    {
        public BackgroundRoll(string name, IRollLogger log, RollAnalyzer roller, List<string> conditions,
            string addtionalInfo, Verbosity verbosity)
            : base(name, log, roller, new List<string>() {name}, true, true, conditions, addtionalInfo, verbosity)
        {
            Conditions.Add(Build.Conditions.Background);
        }

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