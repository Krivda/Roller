using System;
using System.Collections.Generic;
using NLog;

namespace RollerEngine.Character.Modifiers
{
    public enum DurationType
    {
        Roll,
        Scene
    }

    public abstract class ARollModifer
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string Name { get; }
        public int Value { get; }

        public List<string> Traits { get; }
        public DurationType Duration { get; }
        private List<string> Condtions { get; }

        protected ARollModifer(string name, List<string> traits, DurationType duration, List<string> condtions, int value)
        {
            Name = name;
            Value = value;
            Traits = traits;
            Duration = duration;
            Condtions = condtions;
        }

        public bool ConditionsMet(List<string> condtions)
        {
            bool conditionsMet = false;
            if (condtions.Count == 0)
            {
                conditionsMet = true;
            }
            else
            {
                foreach (var condtion in condtions)
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