using System;
using System.Collections.Generic;
using RollerEngine.Character;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    class InstructionTeach : SkillRoll
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
            int result = base.Roll(actor, new List<Build>() { target }, hasSpec, hasWill);

            if (result > 0)
            {
                string traitXpName = Build.DynamicTraits.GetKey(Build.DynamicTraits.Expirience, ability);

                int currentXp;
                int currentLimit;
                if (!target.InstructionXp.ContainsKey(traitXpName))
                {
                    currentXp = 0;
                    currentLimit = 0;
                }
                else
                {
                    currentXp = target.InstructionXp[traitXpName].Item1;
                    currentLimit = target.InstructionXp[traitXpName].Item2;
                }


                int newXP = currentXp + result;


                target.InstructionXp[traitXpName] = new Tuple<int, int>(newXP, actor.Traits[ability]);

                _log.Log(Verbosity.Important, string.Format("{0} got new {1} ({2} in total) bonus XP to spent on dice on {3} rolls from {4} Instruction by {5}.", target.Name, result, newXP, ability, Name, actor.Name));
            }
            else
            {
                _log.Log(Verbosity.Important, string.Format("{0} didn't get bonus XP from {1}.", target.Name, Name));
            }

            return result;
        }
    }
}
