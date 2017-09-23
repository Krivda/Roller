using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Fetish
{
    class CarnyxOfVictory : FetishRoll
    {
        public const string NAME = "Carnyx of Victory";
        public CarnyxOfVictory(
            IRollLogger log,
            IRoller roller,
            Verbosity verbosity) :
            base(
                NAME,
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
                var carnyx = target.BonusDicePoolModifiers.Find(m => m.Name.Equals(NAME));
                if (carnyx != null)
                {
                    target.BonusDicePoolModifiers.Remove(carnyx);
                }
            }
        }

        public new int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            int result = base.Roll(actor, targets, hasSpec, hasWill);
            if (result <= 0)
            {
                Log.Log(Verbosity.Important,
                    string.Format("Party didn't get bonus from {0} fetish.", Name));
                return result;
            }

            foreach (var target in targets)
            {
                if (!target.CheckBonusExists(null, NAME))
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

                    Log.Log(Verbosity, string.Format("{0} obtained {1} dice to his next roll from {2} fetish power .", target.Name, result, Name));
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