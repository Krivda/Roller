using System;
using System.Collections.Generic;
using System.Linq;
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
            new List<string>() {Build.Conditions.Learning, Build.Conditions.Social}, null, Verbosity.Important)
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
                        if (prop.GetValue(null).ToString().Equals(trait))
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

            //TODO: PIZDA dirty hack to learn gift
            if (DicePool.Contains(Build.Abilities.VisageOfFenris))
            {
                increaseDC = true;
            }

            if (increaseDC)
            {
                int dc = 8;
                Log.Log(Verbosity.Important, ActivityChannel.TeachLearn, string.Format("{0} will roll learning ({1}) as untrained at DC {2}", actor.Name, learningTrait, dc));
                return dc;
            }
                

            return base.GetBaseDC(actor, targets);
        }

        public int Roll(Build actor, string ability, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = ability;

            Verbosity saved = Verbosity;
            Verbosity = Verbosity.Critical;
            int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);
            Verbosity = saved;

            if (result > 0)
            {
                string traitKeyXpPool = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpiriencePool, ability);
                string traitKeyXpLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, ability);

                //create traits if they were absent
                if (!actor.Traits.ContainsKey(traitKeyXpPool))
                {
                    actor.Traits.Add(traitKeyXpPool, 0);
                }
                if (!actor.Traits.ContainsKey(traitKeyXpLearned))
                {
                    actor.Traits.Add(traitKeyXpLearned, 0);
                }

                //this is amount xp consumed from xp pool by this roll
                int xpPoolInitial = actor.Traits[traitKeyXpPool];
                int xpConsumedFromPool = Math.Min(result, xpPoolInitial);

                //MAIN: move xp from pool to learned
                actor.Traits[traitKeyXpPool] = actor.Traits[traitKeyXpPool] - xpConsumedFromPool;
                actor.Traits[traitKeyXpLearned] = actor.Traits[traitKeyXpLearned] + xpConsumedFromPool;

                Log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} rolls {1} successes on learning {2} [{3}XP was XP in pool, {4}XP was consumed]",
                    actor.Name, result, ability, xpPoolInitial, xpConsumedFromPool));

                //this is an amount of xp we have learned
                int xpLearned = actor.Traits[traitKeyXpLearned];

                //this is an amount of xp we'll spend to increase trait value
                int xpSpentIncreasingTrait = 0;
                int xpCost = 0;

                int currentTraitValue = actor.Traits[ability];
                for (int i = currentTraitValue+1; i < 6; i++)
                {
                    xpCost = Build.GetSkillXpTable()[i];

                    if (xpCost > xpLearned)
                    {
                        Log.Log(Verbosity.Important, ActivityChannel.TeachLearn, string.Format("{0} don't yet have {1}XP in {2} to increase it's value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool, total {6}XP spent.", 
                            actor.Name, xpCost, ability, i, actor.Traits[traitKeyXpPool], xpLearned, xpSpentIncreasingTrait));
                        break;
                    }

                    xpLearned -= xpCost;
                    xpSpentIncreasingTrait += xpCost;
                    actor.Traits[ability] = actor.Traits[ability] + 1; //TODO check i vs. actor.Traits[ability] consistency (should be ok)
                    Log.Log(Verbosity.Important, ActivityChannel.TeachLearn, string.Format("{0} spent {1} bonus XP on {2}, increasing it's value to {3}. {4} bonus XP remaining in pool, {5}XP learned pool, total {6}XP spent.",
                        actor.Name, xpCost, ability, actor.Traits[ability], actor.Traits[traitKeyXpPool], xpLearned, xpSpentIncreasingTrait));
                }

                //clear xp if trait is raised to 5
                if (actor.Traits[ability] == 5)
                {
                    actor.Traits[traitKeyXpPool] = 0;
                    actor.Traits[traitKeyXpLearned] = 0;
                    Log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} maxed his ability {1}. Remaining XP has burned out.", actor.Name, ability));
                }
                else
                {
                    if (xpSpentIncreasingTrait != 0)
                    {
                        actor.Traits[traitKeyXpLearned] = actor.Traits[traitKeyXpLearned] - xpSpentIncreasingTrait;
                    }
                    Log.Log(Verbosity.Critical, ActivityChannel.TeachLearn, string.Format("{0} spent +{1} bonus XP (total {2}) on {3}, {4}XP remains learned, ({5} bonus XP remains in pool).",
                        actor.Name, xpCost, xpSpentIncreasingTrait, ability, actor.Traits[traitKeyXpLearned], actor.Traits[traitKeyXpPool]));
                }
            }
            else
            {
                //TODO: burn pool on botch?
                Log.Log(Verbosity.Important, ActivityChannel.TeachLearn, string.Format("{0} didn't learn anything.", actor.Name));
            }

            return result;
        }
    }
}