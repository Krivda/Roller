using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class CallOfTheWyldDirgeToTheFallen : GiftRoll
    {
        /*
         * Call of the Wyld:	Stamina + Empathy vs. 6
                        Howl of Introduction
            Howls:			Dirge for the Fallen
                        Call for Succor

            Intents:	impress ancestor spirits 		=> boost Ritual dice pool of rites
                    find best suited ancestor spirits	=> boost Occult dice pool of Ghost Dance
            Ceremony:		+1 dice applied (motivations and desires)
            Sanctified Rosemary: 	vs.4 //because recall facts and experience				//sanctified rosemary

         */

        private const string GIFT_NAME = "Call to Wyld";
        private const string GIFT_NAME_FULL = "Call to Wyld (Dirge to the Fallen)";

        public CallOfTheWyldDirgeToTheFallen(IRollLogger log, IRollAnalyzer roller) : 
            base(GIFT_NAME_FULL, log, roller, 
            new List<string>() { Build.Atributes.Stamina, Build.Abilities.Empathy},
            new List<string>() {Build.Conditions.Memory}, null, Verbosity.Details)
        {
        }

        public int Roll(Build actor, List<Build> targets, string skill, bool hasSpec, bool hasWill)
        {
            int successes = base.Roll(actor, targets, hasSpec, hasWill);

            if (successes > 0)
            {
                foreach (var target in targets)
                {
                    target.TraitModifiers.Add(
                        new TraitModifier(
                            GIFT_NAME,
                            new List<string>() { skill },
                            DurationType.Roll,
                            new List<string>() { Build.Conditions.AncestorSpirits },
                            successes / 2,
                            TraitModifier.BonusTypeKind.AdditionalDice
                        ));

                    Log.Log(Verbosity, string.Format("{0} obtained bonus {1} dices to {2} (Ancestors Spririts related rolls) for next roll from {3} gift performed by {4}.", target.Name, skill, successes / 2, Name, actor.Name));
                }
            }
            else
            {
                Log.Log(Verbosity, string.Format("{0} didn't get bonus from {1} gift.", actor.Name, Name));
            }

            return successes/2;
        }

    }
}