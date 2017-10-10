using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Rites;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Character.Party
{
    public class Yoki : HatysPartyMember
    {

        public Yoki(Build build, IRollLogger log, RollAnalyzer roller, HatysParty party) : base("Йоки", build, log, roller, party)
        {
            //todo: check for arc 7
            WeeklyPartialActions = 4;
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

        public override bool HasSpecOnRite(Rite rite)
        {
            return rite.Info().Group == RiteGroup.Mystic;
        }
    }
}