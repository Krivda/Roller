using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class TeachersEase : GiftRoll
    {
        private const string GIFT_NAME = "Teacher's Ease";

        public TeachersEase(IRollLogger log, RollAnalyzer roller, Verbosity verbosity) :
            base(GIFT_NAME, log, roller, new List<string>() { Build.Atributes.Manipulation, Build.Abilities.Instruction}, new List<string>(){Build.Conditions.Social}, null, verbosity)
        {
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return Math.Max(10 - targets[0].GetModifiedTrait(Build.Atributes.Intellect), 3);
        }

        public int Roll(Build actor, Build target, string ability, bool hasSpec, bool hasWill)
        {
            AdditionalInfo = ability;

            int result = base.Roll(actor, new List<Build>() { target }, hasSpec, hasWill);

            if (result > 0)
            {
                target.TraitModifiers.Add(
                    //string name, List<string> traits, DurationType duration, List<string> condtions, int value, BonusTypeKind bonusType
                    new TraitModifier(
                        Name,
                        new List<string>() { ability }, 
                        DurationType.Roll, 
                        new List<string>(), 
                        result,
                        TraitModifier.BonusTypeKind.AdditionalDice
                    ));

                Log.Log(Verbosity, string.Format("{0} obtained bonus {1} dice on {2} rolls from {3} gift.", target.Name, result, ability, Name));
            }
            else
            {
                Log.Log(Verbosity, string.Format("{0} didn't get bonus from {1} gift.", target.Name, Name));
            }

            return result;
        }

        protected override int OnBotch(int successes)
        {
            return successes;
        }
    }
}