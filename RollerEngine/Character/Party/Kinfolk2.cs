using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;

namespace RollerEngine.Character.Party
{
    public class Kinfolk2 : HatysPartyMember
    {
        public Kinfolk2(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Кинфолк 2", build, log, roller, party)
        {
        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Self, false, true);
        }
    }
}