using System;
using System.Collections.Generic;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    public class InstructionTeach : SkillRoll
    {
        private const string SKILL_NAME = "Instruction (teach)";

        public InstructionTeach(IRollLogger log, IRoller roller) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { Build.Atributes.Manipulation, Build.Abilities.Instruction },
            new List<string>())
        {

        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return Math.Max(9 - targets[0].Traits[Build.Atributes.Intellect], 3);
        }

        public int Roll(Build actor, Build target, string ability, bool hasSpec, bool hasWill)
        {
            int actorTraitValue = actor.Traits[ability];
            int actorInstructAbility = actor.Traits[Build.Abilities.Instruction];

            int maxTeachValue = Math.Min(actorInstructAbility, actorTraitValue);

            int targetTraitValue = target.Traits[ability];

            if (maxTeachValue <= targetTraitValue)
            {
                _log.Log(Verbosity.Important, string.Format("{0} doesn't have more skill in {1} or {2} ability to teach {3}!", actor.Name, target.Name, actorInstructAbility, ability));
                return 0;
            }
            
            int result = base.Roll(actor, new List<Build>() { target }, hasSpec, hasWill);

            if (result > 0)
            {
                string traitXpConsumed = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, ability);
                int alreadyLearned;
                if (!target.Traits.ContainsKey(traitXpConsumed))
                {
                    target.Traits.Add(traitXpConsumed, 0);
                    alreadyLearned = 0;
                }
                else
                {
                    alreadyLearned= target.Traits[traitXpConsumed];
                }

                string traitNameXpInPool = Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceToLearn, ability);
                
                int currentXpInPool;
                if (!target.Traits.ContainsKey(traitNameXpInPool))
                {
                    currentXpInPool = 0;
                    target.Traits.Add(traitNameXpInPool, 0);
                }
                else
                {
                    currentXpInPool = target.Traits[traitNameXpInPool];
                }

                int maxXpPoolFromThisTeacher = 0;

                //calc total possible 
                for (int i = targetTraitValue + 1; i < maxTeachValue + 1; i++)
                {
                    maxXpPoolFromThisTeacher += Build.GetSkillXpTable()[i];
                }

                //redice max XP teacher can teach by already learned and already in pool
                maxXpPoolFromThisTeacher = maxXpPoolFromThisTeacher - alreadyLearned - currentXpInPool;

                //limit XP by teacher's capacity
                int newXp = Math.Min(result, maxXpPoolFromThisTeacher);

                target.Traits[traitNameXpInPool] = currentXpInPool + newXp;

                _log.Log(Verbosity.Important, string.Format("{0} got new {1}XP ({2}XP in total in bonus XP pool) to spent on learning {3} from {4}'s {5}.", target.Name, result, newXp, ability, actor.Name, Name));
            }
            else
            {
                _log.Log(Verbosity.Important, string.Format("{0} didn't get bonus XP from {1}.", target.Name, Name));
            }

            return result;
        }
    }
}
