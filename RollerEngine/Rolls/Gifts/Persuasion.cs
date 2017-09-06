using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class Persuasion : GiftRoll
    {
        private const string GIFT_NAME = "Persuasion";

        public Persuasion(IRollLogger log, IRoller roller) : base(GIFT_NAME, log, roller, new List<string>(){Build.Atributes.Charisma, Build.Abilities.Subterfuge}, new List<string>())
        {
        }

        public int Roll(Build actor, bool hasSpec, bool hasWill)
        {
            if (!actor.CheckBonusExists(Build.Atributes.Charisma, Name))
            {

                int result = base.Roll(actor, new List<Build>() {actor}, hasSpec, hasWill);

                if (result > 0)
                {
                    actor.DCModifiers.Add(
                        new DCModifer(
                            Name,
                            new List<string>()
                            {
                                Build.Atributes.Appearance,
                                Build.Atributes.Charisma,
                                Build.Atributes.Manipulation
                            },
                            DurationType.Scene,
                            new List<string>() {Build.Conditions.Social},
                            -1
                        ));

                    _log.Log(Verbosity.Important,
                    string.Format("{0} obtained bonus -1 DC on social rolls from {1} gift.", actor.Name, Name));
                }
                else
                {
                    _log.Log(Verbosity.Important,
                    string.Format("{0} didn't get bonus from {1} gift.", actor.Name, Name));
                }

                return result;
            }

            return 0;
        }
    }
}