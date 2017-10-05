using System.Collections.Generic;

namespace RollerEngine.Modifiers
{
    public class BonusModifier : ARollModifer
    {
        public BonusModifier(string name, DurationType duration, List<string> conditions, int value) : base(name, null, duration, conditions, value)
        {
        }

        public BonusModifier(string name, DurationType duration, List<string> conditions, List<string> ignoredConditions, int value) : base(name, null, duration, conditions, ignoredConditions, value)
        {
        }


    }
}