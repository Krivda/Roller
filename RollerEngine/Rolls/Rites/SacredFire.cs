using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Rites
{
    public class SacredFire : RiteRoll
    {
        private const string RITE_NAME = "Sacred Fire";

        public SacredFire(IRollLogger log, IRoller roller) : 
            base(RITE_NAME, log, roller,
            new List<string>() { Build.Atributes.Wits, Build.Abilities.Rituals },
            new List<string>() { Build.Conditions.MysticRite }, null, Verbosity.Details)
        {
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //should be always 3
            return 3;
        }

        public new int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            if (!actor.CheckBonusExists(Build.Atributes.Charisma, Name))
            {

                int result = base.Roll(actor, new List<Build>() { actor }, hasSpec, hasWill);

                if (result > 0)
                {
                    result = (result - 1) / 2;

                    //can't exceed 5
                    result = Math.Min(5, result);

                    foreach (var target in targets)
                    {
                        target.DCModifiers.Add(
                            new DCModifer(
                                Name,
                                new List<string>(){ Build.Abilities.Rituals},
                                DurationType.Roll, //TODO WARNING sacred fire lasts for as long as it is tended with sanctified materials
                                new List<string>() {Build.Conditions.MysticRite, Build.Conditions.SpiritRite},
                                -result 
                            ));

                        Log.Log(Verbosity, string.Format("{0} obtained -{1}DC on Mystic/Spirit-related rites from {2} rite .", target.Name, result, Name));
                    }
                }
                else
                {
                    Log.Log(Verbosity.Important,
                        string.Format("{0} didn't get bonus from {1} rite.", "Anybody", Name));
                }

                return result;
            }

            return 0;
        }
    }
}