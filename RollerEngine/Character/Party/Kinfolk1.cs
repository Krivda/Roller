using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class Kinfolk1 : HatysPartyMember
    {
        
        public Kinfolk1(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Кинфолк 1", build, log, roller, party)
        {
        }

    }
}