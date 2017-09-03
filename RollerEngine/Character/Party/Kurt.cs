using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Character.Party
{
    public class Kurt : HatysPartyMember
    {

        public Kurt(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Курт", build, log, roller, party)
        {
        }
    }
}