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
                string traitNameXpToLearn = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteSuccesses, riteName);

                int required = riteLevel * 10;

/*
                int consumedXp = Math.Min(result, actor.Traits[traitNameXpToLearn]);

                actor.Traits[traitNameXpToLearn] = actor.Traits[traitNameXpToLearn] - consumedXp;
                actor.Traits[traitNameXpLearned] = actor.Traits[traitNameXpLearned] + consumedXp;

                if (!actor.Traits.ContainsKey(traitNameXpLearned))
                {
                    actor.Traits.Add(traitNameXpLearned, 0);
                }

                int xpToSpend = actor.Traits[traitNameXpLearned];
                int currentTraitValue = actor.Traits[ability];

                int spentXp = 0;

                for (int i = currentTraitValue + 1; i < 6; i++)
                {

                    int xpCost = Build.GetSkillXpTable()[i];

                    if (xpCost <= xpToSpend)
                    {
                        xpToSpend -= xpCost;
                        spentXp += xpCost;
                        actor.Traits[ability] = actor.Traits[ability] + 1;
                        Log.Log(Verbosity.Warning, string.Format("{0} spent {1} bonus XP on {2} increasing it's value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability], xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
                    }
                    else
                    {
                        Log.Log(Verbosity.Warning, string.Format("{0} don't yet have {1}XP learned to increase {2} value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability] + 1, xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
                        break;
                    }
                }

                //clear xp if trait is raised to 5
                if (actor.Traits[ability] == 5)
                {
                    actor.Traits[traitNameXpToLearn] = 0;
                    actor.Traits[traitNameXpLearned] = 0;
                    Log.Log(Verbosity.Important, string.Format("{0} maxed his ability {1}. Remaining XP burened out.", actor.Name, ability));
                }
                else
                {
                    if (spentXp != 0)
                    {
                        actor.Traits[traitNameXpLearned] = actor.Traits[traitNameXpLearned] - spentXp;
                    }
                }*/
            }
            else
            {
                Log.Log(Verbosity.Important, string.Format("{0} didn't learn anything.", actor.Name));
            }
            

            return result;
        }
    }
}