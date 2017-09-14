using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    class CaernOfVigilChannelling : GiftRoll
    {
        public const string GiftName = "Caern of Vigil (Chanellig)";

        public CaernOfVigilChannelling(IRollLogger log, IRoller roller)
            : base(GiftName, log, roller,
                new List<string>() {Build.Atributes.Perception, Build.Abilities.Alertness},
                new List<string>(), "", Verbosity.Details)
        {
            
        }

        public int Roll(Build actor, string targetTrait, bool withWill)
        {
            int successes = base.Roll(actor, new List<Build>() { actor }, false, withWill);

            if (successes > 0)
            {
                actor.TraitModifiers.Add(new TraitModifier(
                        GiftName,
                        new List<string>() { targetTrait },
                        DurationType.Scene,
                        new List<string>(),
                        successes,
                        TraitModifier.BonusTypeKind.TraitModLimited,
                        5
                    )
                );

                Log.Log(Verbosity, string.Format("{0} obtained bonus {1} to ability {2} for a scence from {3}.", actor.Name, successes, targetTrait, Name));
            }
            else
            {
                Log.Log(Verbosity, string.Format("{0} didn't get bonus from {1}.", actor.Name, Name));
            }

            return successes;

        }
    }
}