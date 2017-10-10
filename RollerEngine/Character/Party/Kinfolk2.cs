using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;

namespace RollerEngine.Character.Party
{
    public class Kinfolk2 : HatysPartyMember
    {
        public Kinfolk2(Build build, IRollLogger log, RollAnalyzer roller, HatysParty party) : base("Кинфолк 2", build, log, roller, party)
        {
        }

        public void CastPersuasion()
        {
            if (!Self.CheckBonusExists(Build.Atributes.Manipulation, Persuasion.GIFT_NAME))
            {
                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Subterfuge, true, Verbosity.Details);
                //Cast Pesuasion
                var persuasionRoll = new Persuasion(Log, Roller);
                persuasionRoll.Roll(Self, false, false);
            }
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {

            //Cast persuasion before teaching
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }
    }
}