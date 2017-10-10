using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Rolls.Rites
{
    public class CreateFetishRite : RiteRoll
    {
        private int _dc;

        public CreateFetishRite(IRollLogger log, RollAnalyzer roller, List<string> additionalConditions) :
            base(Rite.Fetish, log, roller,
                new List<string>() {Build.Atributes.Wits, Build.Abilities.Rituals},
                new List<string>() {Build.Conditions.MysticRite}, null, Verbosity.Details)
        {
            Conditions.AddRange(additionalConditions);
        }

        public int Roll(Build actor, int fetishLevel, string fetishName, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = fetishName;
            _dc = 4 + fetishLevel;

            int result = base.Roll(actor, new List<Build>(), hasSpec, hasWill);

            Log.Log(Verbosity.Critical, "");
            if (result >= 1)
            {
                actor.RemoveFetishBase(fetishName);

                actor.AddFetish(fetishName);

                Log.Log(Verbosity.Critical, string.Format("{0} has got {1} successes on {2}. Now he has an exellent {3}!",
                    actor.Name, result, Rite.Info().Name, fetishName));
            }
            else
            {
                Log.Log(Verbosity.Error, string.Format("{0} has got {1} successes {2}. Fetish {3} should be recrafted!",
                    actor.Name, result, Rite.Info().Name, fetishName));
            }

            return result;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return _dc;
        }
    }
}