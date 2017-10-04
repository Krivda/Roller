using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Fetish
{
    class CarnyxOfVictory : UsingFetishRoll
    {
        public const string FetishName = "Carnyx of Victory";
        public CarnyxOfVictory(
            IRollLogger log,
            IRoller roller,
            Verbosity verbosity) :
            base(
                FetishName,
                log,
                roller,
                new List<string>() {Build.Atributes.Manipulation, Build.Abilities.Performance},
                new List<string>() {Build.Conditions.Social, },
                null, verbosity)
        {
            
        }

        public static void RemoveFromBuild(List<Build> targets)
        {
            foreach (var target in targets)
            {
                var carnyx = target.BonusDicePoolModifiers.Find(m => m.Name.Equals(FetishName));
                if (carnyx != null)
                {
                    target.BonusDicePoolModifiers.Remove(carnyx);
                }
            }
        }

        public int Roll(Build actor, List<Build> targets, string purpose, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = purpose;

            int result = base.Roll(actor, targets, hasSpec, hasWill);
            if (result <= 0)
            {
                Log.Log(Verbosity.Important, ActivityChannel.Boost,
                    string.Format("Party didn't get bonus from {0} fetish.", Name));
                return result;
            }

            foreach (var target in targets)
            {
                if (!target.CheckBonusExists(null, FetishName))
                {
                    int bonus = result;

                    target.BonusDicePoolModifiers.Add(
                        new BonusModifier(
                            Name,
                            DurationType.Scene,
                            new List<string>() { },
                            new List<string>() { Build.Conditions.Background, Build.Conditions.RollableTrait },
                            bonus
                        ));

                    Log.Log(Verbosity, ActivityChannel.Boost, string.Format("{0} obtained {1} dice to his next roll from {2} fetish power .", target.Name, result, Name));
                }
            }

            return result;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return 8;
        }
    }
}