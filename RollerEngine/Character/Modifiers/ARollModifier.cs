using System;
using System.Collections.Generic;
using NLog;

namespace RollerEngine.Character.Modifiers
{
    public enum DurationType
    {
        Roll,
        Scene, 
        Permanent
    }

    public abstract class ARollModifer
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public string Name { get; private set; }
        public int Value { get; private set; }

        public List<string> Traits { get; private set; }
        public DurationType Duration { get; private set; }
        private List<string> Condtions { get; set; }

        protected ARollModifer(string name, List<string> traits, DurationType duration, List<string> condtions, int value)
        {
            Name = name;
            Value = value;
            Traits = traits;
            Duration = duration;
            Condtions = condtions;
        }

        public bool ConditionsMet(List<string> rollConditions)
        {
            bool conditionsMet = false;
            if (Condtions.Count == 0)
            {
                conditionsMet = true;
            }
            else
            {
                foreach (var condtion in rollConditions)
                {
                    conditionsMet = Condtions.Contains(condtion);
                    if (conditionsMet)
                        break;
                }
            }
            return conditionsMet;
        }
    }
}