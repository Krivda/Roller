using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Rolls.Rites
{
    public class SacredFire : RiteRoll
    {
        private readonly int _gnosisSpent = 1;

        public SacredFire(IRollLogger log, RollAnalyzer roller, int additionalGnosisSpent) :
            base(Rite.SacredFire, log, roller,
                null, //default
                new List<string>() {Build.Conditions.MysticRite}, null, Verbosity.Details)
        {
            _gnosisSpent += additionalGnosisSpent;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //Gauntlet; each additional Gnosis spent reduce DC by one
            //todo: if (Environment.Place == Environemnt.Caern) return 2;
            //todo: DC -= (_gnosisSpent - 1);
            return 3;
        }

        public new int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            //todo: check for a need to reroll if not -5
            //todo: general remove/add bonus logic
            if (!actor.CheckBonusExists(Build.Abilities.Rituals, Name))
            {
                actor.SpendGnosis(_gnosisSpent);
                actor.SpendSanctifiedPlant(Build.Counters.SanctifiedPlants.Tobacco,1);

                int result = base.Roll(actor, new List<Build>() {actor}, hasSpec, hasWill);

                if (result > 0)
                {
                    result = (result - 1) / 2;

                    //todo: remove when will do genreal check, can't exceed 5
                    result = Math.Min(5, result);

                    foreach (var target in targets)
                    {
                        target.DCModifiers.Add(
                            new DCModifer(
                                Name,
                                new List<string>() {Build.Abilities.Rituals},
                                DurationType.Roll, //TODO WARNING sacred fire lasts for as long as it is tended with sanctified materials (including tobacco)
                                new List<string>() {Build.Conditions.MysticRite, Build.Conditions.SpiritRite},
                                -result
                            ));

                        Log.Log(Verbosity,
                            string.Format("{0} obtained -{1}DC on Mystic/Spirit-related rites from {2} rite .",
                                target.Name, result, Name));
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