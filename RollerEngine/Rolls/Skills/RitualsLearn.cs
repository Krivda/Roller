using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    public class RitualsLearn : SkillRoll
    {
        private const string SKILL_NAME = "Rite (learn)";

        public RitualsLearn(IRollLogger log, IRoller roller, List<String> conditions) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { Build.Atributes.Intellect, Build.Abilities.Rituals },
            conditions)
        {

        }

        public int Roll(Build actor, string riteName, int riteLevel, bool hasSpec, bool hasWill)
        {
            int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

            if (result > 0)
            {
                string keyRiteName = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteSuccesses, riteName);

                if (!actor.Traits.ContainsKey(keyRiteName))
                {
                    actor.Traits.Add(keyRiteName,0);
                }

                int successesAlreadyTowardsRite = actor.Traits[keyRiteName];
                int successesRequired = riteLevel * 10;
                int successesTotal = successesAlreadyTowardsRite + result;

                if (successesRequired > successesAlreadyTowardsRite + result)
                {
                    //not enought yet!
                    Log.Log(Verbosity.Warning, string.Format("{0} has advanced in learning rite {1} on {2} more success. Now he has {3} of {4} successes!",
                        actor.Name, riteName, result, successesTotal, successesRequired));
                }
                else
                {
                    successesTotal = 0;
                    Log.Log(Verbosity.Warning, string.Format("{0} has finally learned rite {1}!",
                        actor.Name, riteName));
                }

                actor.Traits[keyRiteName] = successesTotal;

            }
            else
            {
                Log.Log(Verbosity.Warning, string.Format("{0} didn't learn anything.", actor.Name));
            }
            

            return result;
        }
    }
}