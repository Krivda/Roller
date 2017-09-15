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
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Build, false, true);
        }

        public void CastVisageOfFenris()
        {
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
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }

        public void CastSacredFire()
        {
            var sacredFire = new SacredFire(Log, Roller);
            sacredFire.Roll(Build,
                new List<Build>() {Party.Spiridon.Build, Party.Nameless.Build, Party.Yoki.Build, Party.Kurt.Build}, true,
                false);
        }

        public void WeeklyBoostSkill(string trait)
        {
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Build, Log);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Occult
            ApplyAncestors(Build.Abilities.Occult);

            Nameless nameless = Party.Nameless;

            nameless.CastTeachersEase(Build, Build.Abilities.Rituals, true, Verbosity.Details);
            CastSacredFire();

            
            CastCallToWyld(new List<Build>() { nameless.Build, Party.Yoki.Build, Party.Kurt.Build, Build }, Build.Abilities.Occult);

            CastVisageOfFenris();
            
            CastPersuasion();

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //cast rite of Anscestor Seeking
            CaseAnscestorSeeking();

            //roll Ghost Pack
            var ghostPackRoll = new GhostPack(Log, Roller);
            ghostPackRoll.Roll(Build, false, false);

            //Apply Bone Ryhtms
            CommonBuffs.ApplyBoneRythms(Build, Log);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Instruct 

            Build.UsedAncestorsCount = Build.UsedAncestorsCount - 1;
            ApplyAncestors(trait);

        }

        private void CaseAnscestorSeeking()
        {
            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Build, false, true);
        }
    }
}