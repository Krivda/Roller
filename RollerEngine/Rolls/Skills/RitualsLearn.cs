using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.Rolls.Skills
{
    public class RitualsLearn : SkillRoll
    {
        private const string SKILL_NAME = "Rite (learn)";

        public RitualsLearn(IRollLogger log, IRoller roller) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { Build.Atributes.Intellect, Build.Abilities.Rituals },
            new List<string>() { Build.Conditions.LearningRites }, 
            null, Verbosity.Important)
        {
        }

        public int Roll(Build actor, string riteName, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = riteName;

            int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

            if (result > 0)
            {
                string keyRitePool = Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, riteName);
                string keyRiteLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned, riteName);
              
                //create dynamic trait if it was absent
                if (!actor.Traits.ContainsKey(keyRitePool))
                {
                    RiteInfo rite;

                    if (! RitesDictionary.Rites.TryGetValue(riteName, out rite))
                    {
                        throw new Exception(string.Format("Rite {0} is not known by software!", riteName));
                    }

                    actor.Traits.Add(keyRitePool, rite.Level * 10);
                }

                //create dynami trait if it was absent
                if (!actor.Traits.ContainsKey(keyRiteLearned))
                {
                    actor.Traits.Add(keyRiteLearned, 0);
                }

                //throw if rite was already learned
                if (actor.IsRiteLearned(riteName))
                {
                    throw new Exception(string.Format("Rite {0} was already learned by {1}", riteName, actor.Name));
                }

                int successesInitial = actor.Traits[keyRiteLearned];
                int successesRequired = actor.Traits[keyRitePool];
                int successesLearned = successesInitial + result;

                if (successesRequired > successesLearned)
                {
                    //not enought yet!
                    Log.Log(Verbosity, string.Format("{0} has advanced in learning rite {1} on {2} more success. Now he has {3} of {4} successes!",
                        actor.Name, riteName, result, successesLearned, successesRequired));
                }
                else
                {
                    successesLearned = Build.RiteAlreadyLearned;
                    Log.Log(Verbosity, string.Format("{0} has finally learned rite {1}!",
                        actor.Name, riteName));
                }

                actor.Traits[keyRiteLearned] = successesLearned;
            }
            else
            {
                Log.Log(Verbosity, string.Format("{0} didn't learn anything.", actor.Name));
            }
            
            return result;
        }
    }
}