using System.Collections.Generic;

namespace RollerEngine.Character.Modifiers
{
    // ReSharper disable once InconsistentNaming
    public class DCModifer : ARollModifer
    {
        public DCModifer(string name, List<string> traits, DurationType duration, List<string> condtions, int value) : 
            base(name, traits, duration, condtions, value)
        {
        }
    }
}