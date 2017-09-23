using System;
using System.Collections.Generic;
using NLog;
using RollerEngine.Rolls;

namespace RollerEngine.Modifiers
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
        private List<string> Conditions { get; set; }
        private List<string> IgnoredConditions { get; set; }

        protected ARollModifer(string name, List<string> traits, DurationType duration, List<string> conditions, List<string> ignoredConditions, int value)
        {
            Name = name;
            Value = value;
            Traits = traits;
            Duration = duration;
            Conditions = conditions;
            IgnoredConditions = ignoredConditions;
        }

        protected ARollModifer(string name, List<string> traits, DurationType duration, List<string> conditions, int value) :  this (name, traits, duration, conditions, new List<string>(), value)
        {

        }

        public virtual bool ConditionsMet(RollBase roll)
        {
            if (Conditions.Count == 0)
            {
                if (IgnoredConditions.Count != 0)
                {
                    foreach (var condtion in roll.Conditions)
                    {
                        var found = IgnoredConditions.Contains(condtion);
                        if (found)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            foreach (var condtion in roll.Conditions)
            {
                var found = IgnoredConditions.Contains(condtion);
                if (found)
                {
                    return false;
                }

                found = Conditions.Contains(condtion);
                if (found)
                {
                    return true;
                }

            }

            return false;
        }
    }
}