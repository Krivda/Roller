using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Skills
{
    public class CraftFetishBase : SkillRoll
    {
        private const string SKILL_NAME = "Craft fetish base";

        private int _dc;

        public CraftFetishBase(IRollLogger log, IRoller roller) : base(
            SKILL_NAME,
            log,
            roller,
            new List<string>() { Build.Atributes.Dexterity, Build.Abilities.Crafts},
            new List<string>() {  }, 
            null, Verbosity.Important)
        {
        }

        public int Roll(Build actor, int fetishLevel, string fetishName, bool hasSpec, bool hasWill)
        {
            int successesRequires = fetishLevel;
            _dc = 3 + fetishLevel;
            AdditionalInfo = fetishName;

            int result = base.Roll(actor, new List<Build>(), hasSpec, hasWill);

            if (result >= successesRequires)
            {
                actor.Items.Add(string.Format("base for fetish {0}", fetishName), 1);

                Log.Log(Verbosity.Critical, ActivityChannel.Creation, string.Format("{0} has got {1} successes (of required {2}) on {3}. Now he has an exellent base for {4}!",
                    actor.Name, result, successesRequires, SKILL_NAME, fetishName));
            }
            else
            {
                Log.Log(Verbosity.Critical, ActivityChannel.Creation, string.Format("{0} has got {1} successes (of required {2}) on {3}. Thats not enough to create a decent base for {4}!",
                    actor.Name, result, successesRequires, SKILL_NAME, fetishName));
            }

            return result;
        }


        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return _dc;
        }
    }
}