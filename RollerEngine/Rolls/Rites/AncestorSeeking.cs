using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Rites
{
    class AncestorSeeking : RiteRoll
    {
        private const string RITE_NAME = "Ancestor Seeking";

        public AncestorSeeking(IRollLogger log, IRoller roller) : base(RITE_NAME, log, roller, 
            new List<string>() {Build.Atributes.Wits, Build.Abilities.Rituals },
            new List<string>() {Build.Conditions.MysticRite}, null, Verbosity.Details) 
        {
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return 7;
        }

        public int Roll(Build actor, Build target, bool hasSpec, bool hasWill)
        {
            if (!target.CheckBonusExists(Build.Atributes.Charisma, Name))
            {

                int result = base.Roll(actor, new List<Build>() { target }, hasSpec, hasWill);

                if (result > 0)
                {
                    target.TraitModifiers.Add(
                        new TraitModifier(
                            Name,
                            new List<string>()
                            {
                                Build.Abilities.Occult,
                                Build.Abilities.Enigmas,
                                Build.Abilities.Investigation
                            },
                            DurationType.Roll,
                            new List<string>(),
                            result/2,
                            TraitModifier.BonusTypeKind.AdditionalDice
                        ));

                    Log.Log(Verbosity, ActivityChannel.Intermediate,
                    string.Format("{0} obtained bonus {1} dice on social rolls from {2} rite to Occult, Enigmas, Investigation for next Ancestor spirit related rolls.", target.Name, result/2, Name));
                }
                else
                {
                    Log.Log(Verbosity, ActivityChannel.Intermediate,
                    string.Format("{0} didn't get bonus from {1} rite.", target.Name, Name));
                }

                return result/2;
            }

            return 0;
        }
    }
}