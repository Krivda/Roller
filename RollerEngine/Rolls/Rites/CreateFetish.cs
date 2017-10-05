using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Rites
{
    public class CreateFetishRite : RiteRoll
    {
        private readonly RiteInfo _rite;
        private int _dc;

        public CreateFetishRite(IRollLogger log, IRoller roller, List<string> additionalConditions) :
            base(RitesDictionary.Rites[Rite.Fetish].Name, log, roller,
                new List<string>() {Build.Atributes.Wits, Build.Abilities.Rituals},
                new List<string>() {Build.Conditions.MysticRite}, null, Verbosity.Details)
        {
            Conditions.AddRange(additionalConditions);
            _rite = RitesDictionary.Rites[Rite.Fetish];
        }

        public int Roll(Build actor, int fetishLevel, string fetishName, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = fetishName;
            _dc = 4 + fetishLevel;

            int result = base.Roll(actor, new List<Build>(), hasSpec, hasWill);

            Log.Log(Verbosity.Critical, ActivityChannel.Creation, "");
            if (result >= 1)
            {
                actor.RemoveFetishBase(fetishName);

                actor.AddFetish(fetishName);

                Log.Log(Verbosity.Critical, ActivityChannel.Creation, string.Format("{0} has got {1} successes on {2}. Now he has an exellent {3}!",
                    actor.Name, result, _rite.Name, fetishName));
            }
            else
            {
                Log.Log(Verbosity.Error, ActivityChannel.Creation, string.Format("{0} has got {1} successes {2}. Fetish {3} should be recrafted!",
                    actor.Name, result, _rite.Name, fetishName));
            }

            return result;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return _dc;
        }
    }
}