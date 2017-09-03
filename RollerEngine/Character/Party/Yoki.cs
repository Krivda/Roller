using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class Yoki : HatysPartyMember
    {
 
        public Yoki(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Йоки", build, log, roller, party)
        {
        }

    }
}