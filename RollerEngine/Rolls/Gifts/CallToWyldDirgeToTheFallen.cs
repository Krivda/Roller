using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class CallToWyldDirgeToTheFallen : GiftRoll
    {
        private const string GIFT_NAME = "Call to Wyld";
        private const string GIFT_NAME_FULL = "Call to Wyld (Dirge to the Fallen)";

        public CallToWyldDirgeToTheFallen(IRollLogger log, IRoller roller) : base(GIFT_NAME_FULL, log, roller, 
            new List<string>() { Build.Atributes.Stamina, Build.Abilities.Empathy},
            new List<string>() { })
        {
        }

        public int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            int successes = base.Roll(actor, targets, hasSpec, hasWill);

            if (successes > 0)
            {
                foreach (var target in targets)
                {
                    target.BonusDicePoolModifiers.Add(
                        new BonusModifier(
                            GIFT_NAME,
                            DurationType.Roll,
                            new List<string>() { Build.Conditions.AncestorSpirits },
                            successes / 2
                        ));

                    _log.Log(Verbosity.Important, string.Format("{0} obtained bonus {1} dices to Ansecetors Spririts related rolls for next roll from {2} gift performed by {3}.", target.Name, successes/2, Name, actor.Name));
                }

            }
            else
            {
                _log.Log(Verbosity.Important, string.Format("{0} didn't get bonus from {1} gift.", actor.Name, Name));
            }

            return successes;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //base DC
            return 7;
        }
    }
}