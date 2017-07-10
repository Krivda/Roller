using System.Collections.Generic;

namespace RollerEngine.Character.Modifiers
{
    public class BonusModifier : ARollModifer
    {
        public BonusModifier(string name, DurationType duration, List<string> condtions, int value) : base(name, null, duration, condtions, value)
        {
        }
    }
}