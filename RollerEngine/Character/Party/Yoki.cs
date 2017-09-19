using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;

namespace RollerEngine.Character.Party
{
    public class Yoki : HatysPartyMember
    {
 
        public Yoki(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Йоки", build, log, roller, party)
        {
            LearnAttempts = 4;
        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Self, false, true);
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {
            //Cast persuasion before teaching
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }
    }
}