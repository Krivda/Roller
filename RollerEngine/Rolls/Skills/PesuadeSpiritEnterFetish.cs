using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    //TODO: another roll variant is 3+spirit's Gnosis is in case of intimidation or other ordering (like Call of Duty)
    public class PesuadeSpiritEnterFetish : SkillRoll
    {
        private const string SKILL_NAME = "Persuade spirit to enter fetish";

        public PesuadeSpiritEnterFetish(IRollLogger log, RollAnalyzer roller, string attribute, string ability, List<string> additionalConditions) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { attribute, ability},
            new List<string>() { Build.Conditions.Social, Build.Conditions.GaianSpiritsSocial }, 
            null, Verbosity.Important)
        {
            Conditions.AddRange(additionalConditions);
        }

        public int Roll(Build actor, string fetishName, string spiritType, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = string.Format("{0} with {1}", fetishName, spiritType);
            int result = base.Roll(actor, new List<Build>(), hasSpec, hasWill);

            if (result >= 1)
            {
                Log.Log(Verbosity.Critical, string.Format("{0} has got {1} successes on {2}. He conviced {3} spirit to enter {4}!",
                    actor.Name, result, SKILL_NAME, spiritType, fetishName));
            }
            else
            {
                Log.Log(Verbosity.Critical, string.Format("{0} has got {1} successes on {2}. {3} spirit REFUSES to enter {4}!",
                    actor.Name, result, SKILL_NAME, spiritType, fetishName));
            }

            return result;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return 8;
        }
    }
}
