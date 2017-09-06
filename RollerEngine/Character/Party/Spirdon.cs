using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;

namespace RollerEngine.Character.Party
{
    public class Spirdon : HatysPartyMember
    {
        public Spirdon(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Спиридон", build, log, roller, party)
        {
        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Build, false, true);
        }

        public void CastCallToWyld(List<Build> target)
        {
            //Nameless buffs Empthy
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Empathy, true);
            
            //Cast Call to Wyld
            var callToWyld = new CallToWyldDirgeToTheFallen(Log, Roller);
            callToWyld.Roll(Build, target, true, false);
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {
            //Cast persuasion before teaching
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }
    }
}