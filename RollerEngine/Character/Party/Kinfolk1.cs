using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;

namespace RollerEngine.Character.Party
{
    public class Kinfolk1 : HatysPartyMember
    {
        
        public Kinfolk1(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Кинфолк 1", build, log, roller, party)
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