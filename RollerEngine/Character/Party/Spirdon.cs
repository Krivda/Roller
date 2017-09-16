using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.Character.Party
{
    public class Spirdon : HatysPartyMember
    {
        public Spirdon(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Спиридон", build, log, roller, party)
        {
        }

        public void CastPersuasion()
        {
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Subterfuge, true, Verbosity.Details);
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Build, false, false);
        }

        public void CastVisageOfFenris()
        {

            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Intimidation, true, Verbosity.Details);
            //Cast Visage of fenfis
            var vizageOfFenris = new VizageOfFenris(Log, Roller);
            vizageOfFenris.Roll(Build, false, false);
        }

        public void CastCallToWyld(List<Build> target, string skill)
        {
            //Nameless buffs Empthy
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Empathy, true, Verbosity.Details);
            
            //Cast Call to Wyld
            var callToWyld = new CallOfTheWyldDirgeToTheFallen(Log, Roller);
            callToWyld.Roll(Build, target, skill, true, false);
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {
            //Cast persuasion before teaching
            CastVisageOfFenris();
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }

        public void CastSacredFire()
        {
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Rituals, true, Verbosity.Details);

            var sacredFire = new SacredFire(Log, Roller);

            sacredFire.Roll(Build, new List<Build>() {Party.Spiridon.Build, Party.Nameless.Build, Party.Yoki.Build, Party.Kurt.Build}, false, false);
        }

        public void WeeklyBoostSkill(string trait)
        {
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Build, Log);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Occult
            ApplyAncestors(Build.Abilities.Occult);

            CastSacredFire();
            
            CastVisageOfFenris();

            CastPersuasion();

            //cast rite of Anscestor Seeking
            CastAnscestorSeeking();

            CastCallToWyld(new List<Build>() { Party.Nameless.Build, Party.Yoki.Build, Party.Kurt.Build, Build }, Build.Abilities.Occult);

            CastGhostPack(trait);
            
        }

        private void CastGhostPack(string trait)
        {
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Occult, false, Verbosity.Details);

            //roll Ghost Pack
            var ghostPackRoll = new GhostPack(Log, Roller);
            ghostPackRoll.Roll(Build, true, false);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            Build.AncestorsUsesLeft = Build.AncestorsUsesLeft - 1;
            ApplyAncestors(trait);

        }

        private void CastAnscestorSeeking()
        {
            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Rituals, false, Verbosity.Details);
            
            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Build, false, true);
        }
    }
}