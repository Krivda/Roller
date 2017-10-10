using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Character.Party
{
    public class Lynn : HatysPartyMember
    {
        public Lynn(Build build, IRollLogger log, RollAnalyzer roller, HatysParty party) : base("Линь", build, log, roller, party)
        {
        }
    }
}