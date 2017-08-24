using System.Collections.Generic;
using RollerEngine.Character;
using RollerEngine.Character.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Backgrounds
{
    class Ancestors : BackgroundRoll
    {

        public Ancestors(ILogger log, IRoller roller)
            : base(Build.Backgrounds.Ansestors, log, roller, new List<string>() {Build.Conditions.AncestorSpirits})
        {}
        

        public int Roll(Build actor, string targetTrait)
        {
            int successes = base.Roll(actor, new List<Build>() {actor}, false, false);

            if (successes > 0)
            {
                actor.TraitModifiers.Add(new TraitModifier(
                    Build.Backgrounds.Ansestors,
                    new List<string>() {targetTrait},
                    DurationType.Scene,
                    new List<string>(),
                    successes,
                    TraitModifier.BonusTypeKind.AdditionalDice,
                    -1
                    )
                );

                _log.Log(Verbosity.Details, string.Format("{0} obtained bonus {1} dies on {2} for a scence from {3} Background.", actor.Name, successes, targetTrait ,Name));
            }
            else
            {
                _log.Log(Verbosity.Details, string.Format("{0} didn't get bonus from {1} Background.", actor.Name, Name));
            }

            return successes;

        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //base DC is 8
            return 8;
        }
    }
}