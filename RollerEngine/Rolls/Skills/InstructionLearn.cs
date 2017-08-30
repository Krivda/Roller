using System;
using System.Collections.Generic;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    public class InstructionLearn : SkillRoll
    {
        private const string SKILL_NAME = "Instruction (learn)";

        public InstructionLearn(IRollLogger log, IRoller roller, string ability) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { Build.Atributes.Intellect, ability},
            new List<string>())
        {

        }


        public int Roll(Build actor, string ability, bool hasSpec, bool hasWill)
        {
            int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

            if (result > 0)
            {
                string traitNameXpToLearn = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceToLearn, ability);
                string traitNameXpLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, ability);

                int consumedXp = Math.Min(result, actor.Traits[traitNameXpToLearn]);

                actor.Traits[traitNameXpToLearn] = actor.Traits[traitNameXpToLearn] - consumedXp;
                actor.Traits[traitNameXpLearned] = actor.Traits[traitNameXpLearned] + consumedXp;

                if (!actor.Traits.ContainsKey(traitNameXpLearned))
                {
                    actor.Traits.Add(traitNameXpLearned,0);
                }

                int xpToSpend = actor.Traits[traitNameXpLearned];
                int currentTraitValue = actor.Traits[ability];

                int spentXp = 0;

                for (int i = currentTraitValue+1; i < 6; i++)
                {

                    int xpCost = Build.GetSkillXpTable()[i];

                    if (xpCost <= xpToSpend)
                    {
                        xpToSpend -= xpCost;
                        spentXp += xpCost;
                        actor.Traits[ability] = actor.Traits[ability] + 1;
                        _log.Log(Verbosity.Important, string.Format("{0} spent {1} bonus XP on {2} increasing it's value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability], xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
                    }
                    else
                    {
                        _log.Log(Verbosity.Important, string.Format("{0} don't yet have {1}XP learned to increase {2} value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability]+1, xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
                        break;
                    }
                }

                //clear xp if trait is raised to 5
                if (actor.Traits[ability] == 5)
                {
                    actor.Traits[traitNameXpToLearn] = 0;
                    actor.Traits[traitNameXpLearned] = 0;
                    _log.Log(Verbosity.Important, string.Format("{0} maxed his ability {1}. Remaining XP burened out.", actor.Name, ability));
                }
                else
                {
                    if (spentXp != 0)
                    {
                        actor.Traits[traitNameXpLearned] = actor.Traits[traitNameXpLearned] - spentXp;
                    }
                }
            }
            else
            {
                _log.Log(Verbosity.Important, string.Format("{0} didn't learn anything.", actor.Name));
            }

            return result;
        }
    }
}