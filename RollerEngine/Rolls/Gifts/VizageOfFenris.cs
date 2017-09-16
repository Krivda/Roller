using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class VizageOfFenris : GiftRoll
    {
        private const string GIFT_NAME = "Visage of Fenfis";

        public VizageOfFenris(IRollLogger log, IRoller roller)
            : base(GIFT_NAME, log, roller, new List<string>() { Build.Atributes.Charisma, Build.Abilities.Intimidation }, new List<string>() {Build.Conditions.Social}, null, Verbosity.Details)
        {
        }

        public int Roll(Build actor, bool hasSpec, bool hasWill)
        {
            if (!actor.CheckBonusExists(null, Name))
            {

                int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

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
                            new List<string>() { Build.Conditions.Social },
                            -1
                        ));

                    Log.Log(Verbosity,
                        string.Format("{0} obtained bonus -1 DC on social rolls from {1} gift.", actor.Name, Name));
                }
                else
                {
                    //botch
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
                            new List<string>() { Build.Conditions.Social },
                            1
                        ));

                    Log.Log(Verbosity.Warning, string.Format("{0} bothced {1} gift and now has -1 DC on social rolls.", actor.Name, Name));
                }

                return result;
            }

            return 0;
        }

        protected override int OnBotch(int successes)
        {
            return successes;
        }
    }
}