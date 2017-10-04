using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class CaernOfVigilChannelling : GiftRoll
    {
        public const string PowerName = "Caern of Vigil (Chanelling)";

        public CaernOfVigilChannelling(IRollLogger log, IRoller roller)
            : base(PowerName, log, roller,
                new List<string>() {Build.Atributes.Perception, Build.Abilities.PrimalUrge},
                new List<string>(), "", Verbosity.Details)
        {
            
        }

        public int Roll(Build actor, string targetTrait, bool withWill)
        {
            AdditionalInfo = targetTrait;

            int successes = base.Roll(actor, new List<Build>() { actor }, false, withWill);

            if (successes > 0)
            {
                actor.TraitModifiers.Add(new TraitModifier(
                        PowerName,
                        new List<string>() { targetTrait },
                        DurationType.Scene,
                        new List<string>(),
                        successes,
                        TraitModifier.BonusTypeKind.TraitModLimited,
                        5
                    )
                );

                Log.Log(Verbosity, ActivityChannel.Intermediate, string.Format("{0} obtained bonus {1} to ability {2} for a scence from {3}.", actor.Name, successes, targetTrait, Name));
            }
            else
            {
                Log.Log(Verbosity, ActivityChannel.Intermediate, string.Format("{0} didn't get bonus from {1}.", actor.Name, Name));
            }

            return successes;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return 7;
        }
    }
}