using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Fetish;
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
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Subterfuge, true, Verbosity.Details);
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Self, false, false);
        }

        public void CastVisageOfFenris()
        {

            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Intimidation, true, Verbosity.Details);
            //Cast Visage of fenfis
            var vizageOfFenris = new VizageOfFenris(Log, Roller);
            vizageOfFenris.Roll(Self, false, false);
        }

        public void CastCallToWyld(List<Build> target, string skill)
        {
            //Nameless buffs Empthy
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Empathy, true, Verbosity.Details);
            
            //Cast Call to Wyld
            var callToWyld = new CallOfTheWyldDirgeToTheFallen(Log, Roller);
            callToWyld.Roll(Self, target, skill, true, false);
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {
            //Cast persuasion before teaching
            CastVisageOfFenris();
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }

        public void CastSacredFire(List<Build> targets)
        {
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, true, Verbosity.Details);

            var sacredFire = new SacredFire(Log, Roller);
            sacredFire.Roll(Self, targets,  false, true);
        }

        public void WeeklyPreBoost(string suppTrait)
        {
            Log.Log(Verbosity.Details, ActivityChannel.Boost, "=== === === === ===");
            Log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} WeeklyPreBoost on {1}", Self.Name, suppTrait));

            ShiftToCrinos();
            //apply heighten sences
            CommonBuffs.ApplyHeightenSenses(Self, Log);
            //-1 dc social rolls
            CastVisageOfFenris();
            //-1 dc social rolls
            CastPersuasion();
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);
            //apply
            CommonBuffs.ApplySacredRosemary(Party.Spiridon.Self, Log); //TODO: this usage should be counted once more because its target are spirits (do when we'll count talens)
            //buff Occult
            ApplyAncestors(suppTrait);
        }

        public void WeeklyMidBoostOccult(Build target)
        {
            Log.Log(Verbosity.Details, ActivityChannel.Boost, "=== === === === ===");
            Log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} WeeklyMidBoostOccult for {1}", Self.Name, target.Name));

            //for my next Ancestor Seeking
            CastSacredFire(new List<Build>() { Self });
            //cast rite of Anscestor Seeking - boost Occult to target
            Party.Spiridon.ActivateCarnyx();
            CastAnscestorSeeking(target);
            Party.Spiridon.DeactivateCarnyx();
            //boost Occult to target
            Party.Spiridon.ActivateCarnyx();
            CastCallToWyld(new List<Build>() { target }, Build.Abilities.Occult);
            Party.Spiridon.DeactivateCarnyx();
        }

        public void WeeklyBoostSkill(string mainTrait)
        {
            Log.Log(Verbosity.Details, ActivityChannel.Boost, "=== === === === ===");
            Log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} WeeklyBoostSkill on {1}", Self.Name, mainTrait));

            //Buff occult from Spiridon
            Party.Spiridon.WeeklyMidBoostOccult(Self);

            //Maximum boost for trait
            Party.Spiridon.ActivateCarnyx();
            CastGhostPack(mainTrait);
            Party.Spiridon.DeactivateCarnyx();
        }

        private void CastGhostPack(string trait)
        {
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Occult, false, Verbosity.Details);

            //roll Ghost Pack
            var ghostPackRoll = new GhostPack(Log, Roller);
            ghostPackRoll.Roll(Self, true, false);

            ApplyAncestors(trait);
        }

        public void CastAnscestorSeeking(Build target)
        {
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);
            
            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Self, target, false, true);
        }

        public void CastMindPartition()
        {
            //Cast Pesuasion
            var mindPartition = new MindPartition(Log, Roller);
            int mindParts = 1 + mindPartition.Roll(Self);
            LearnAttempts *= mindParts; //TODO more complicated (requires reroll of the gift for finished learning activities)
        }

        public void CastCaernChanneling(string trait)
        {
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.PrimalUrge, false, Verbosity.Details);

            var caernChanelling = new CaernOfVigilChannelling(Log, Roller);
            caernChanelling.Roll(Self, trait, false);
        }

        public void ActivateCarnyx()
        {
            Log.Log(Verbosity.Critical, ActivityChannel.Boost, ">== Carnyx started, now actions from Spiridon");
            //TODO: -1 Gnosis to activate

            CastCaernChanneling(Build.Abilities.Performance);
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Performance, false, Verbosity.Details);

            var carnyx = new CarnyxOfVictory(Log, Roller, Verbosity.Critical);
            carnyx.Roll(Self, Party.Builds.FindAll(build => !build.Name.Equals(CharacterName)), false, false);
        }

        public void DeactivateCarnyx()
        {
            CarnyxOfVictory.RemoveFromBuild(Party.Builds.FindAll(build => !build.Name.Equals(CharacterName)));
            Log.Log(Verbosity.Critical, ActivityChannel.Boost, "<== Carnyx ended, Spiridon can act again");
        }
    }
}