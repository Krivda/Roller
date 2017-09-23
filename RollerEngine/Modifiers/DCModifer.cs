using System.Collections.Generic;

namespace RollerEngine.Modifiers
{
    // ReSharper disable once InconsistentNaming
    public class DCModifer : ARollModifer
    {
        public DCModifer(string name, List<string> traits, DurationType duration, List<string> conditions, int value) : 
            base(name, traits, duration, conditions, value)
        {
        }
    }
}