using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
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
            new List<string>() {Build.Conditions.Learning, Build.Conditions.Social})
        {

        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //It's not right, but...

            var target = targets[0];

            string learningTrait = "";

            bool increaseDC = false;
            foreach (var trait in DicePool)
            {
                // do it for only 0-value traits
                if (target.Traits[trait] == 0)
                {
                    //
                    var props = typeof(Build.Abilities).GetFields();
                    foreach (var prop in props)
                    {
                        if (prop.Name.Equals(trait))
                        {
                            //this is a zero ability
                            learningTrait = trait;

                            //check if there's an buff on that ability that will cancel untrained mod.
                            var hasSuitableMod = target.TraitModifiers.Any(modifier =>
                                modifier.Traits.Any(mm => mm.Equals(prop.Name)) &&
                                (modifier.BonusType == TraitModifier.BonusTypeKind.TraitMod || modifier.BonusType == TraitModifier.BonusTypeKind.TraitModLimited) &&
                                modifier.Value > 0);

                            if (!hasSuitableMod)
                            {
                                increaseDC = true;
                                break;
                            }
                        }
                    }

                    if (!increaseDC)
                        break;
                }
            }

            if (increaseDC)
            {
                int dc = 8;
                Log.Log(Verbosity.Warning, string.Format("{0} will roll learning ({1}) as untrained at DC {2}", actor.Name, learningTrait, dc));
                return dc;
            }
                

            return base.GetBaseDC(actor, targets);
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
                        Log.Log(Verbosity.Important, string.Format("{0} spent {1} bonus XP on {2} increasing it's value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability], xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
                    }
                    else
                    {
                        Log.Log(Verbosity.Important, string.Format("{0} don't yet have {1}XP learned to increase {2} value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool.", actor.Name, xpCost, ability, actor.Traits[ability]+1, xpToSpend, actor.Traits[traitNameXpLearned] - spentXp));
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
                }
            }
            else
            {
                Log.Log(Verbosity.Important, string.Format("{0} didn't learn anything.", actor.Name));
            }

            return result;
        }
    }
}