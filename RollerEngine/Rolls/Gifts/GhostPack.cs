using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class GhostPack : GiftRoll
    {
        private const string GIFT_NAME = "Ghost Pack";

        public GhostPack(IRollLogger log, IRoller roller) : base(GIFT_NAME, log, roller, 
            new List<string>() { Build.Atributes.Charisma, Build.Abilities.Occult },
            new List<string>() {Build.Conditions.Social, Build.Conditions.AncestorSpirits, Build.Conditions.SpiritHeritage})
        {
        }

        public int Roll(Build actor, bool hasSpec, bool hasWill)
        {
            int successes = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

            if (successes > 0)
            {
                actor.TraitModifiers.Add(
                    new TraitModifier(
                        Name,
                        new List<string>() { Build.Backgrounds.Ansestors },
                        DurationType.Roll,
                        new List<string>(),
                        successes, TraitModifier.BonusTypeKind.AdditionalDice, -1
                    ));

                _log.Log(Verbosity.Important, string.Format("{0} obtained bonus {1} to Ancestors from {2} gift.", actor.Name, successes, Name));
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