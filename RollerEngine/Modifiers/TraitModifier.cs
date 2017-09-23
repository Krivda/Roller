using System.Collections.Generic;
using NLog;
using RollerEngine.Character.Common;

namespace RollerEngine.Modifiers
{
    public class TraitModifier : ARollModifer
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public enum BonusTypeKind
        {
            TraitMod = 1,
            TraitModLimited = 2,
            AdditionalDice = 3
        }

        public BonusTypeKind BonusType { get; private set; }
        public int Limit { get; private set; }

        public TraitModifier(string name, List<string> traits, DurationType duration, List<string> conditions, int value, BonusTypeKind bonusType) : base(name, traits, duration, conditions, value)
        {
            BonusType = bonusType;
            Limit = -1;
        }

        public TraitModifier(string name, List<string> traits, DurationType duration, List<string> conditions, int value, BonusTypeKind bonusType, int limit) : base(name, traits, duration, conditions, value)
        {
            BonusType = bonusType;
            Limit = limit;
        }

        public int GetLimitedValue(Build build, int currentValue)
        {
            int bonus = Value;

            if (Limit != -1)
            {
                int maxValue =  bonus + currentValue;
                if (maxValue > Limit)
                    maxValue = Limit;

                bonus = maxValue - currentValue;

                if (bonus < 0)
                    bonus = 0;
            }

            return bonus;
        }
    }
}