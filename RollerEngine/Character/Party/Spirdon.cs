using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Fetish;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Rites;
using RollerEngine.Rolls.Skills;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Character.Party
{
    public class Spirdon : HatysPartyMember
    {
        public bool HasCarnyx
        {
            get
            {
                return Self.Items.ContainsKey(CarnyxOfVictory.FetishName);
            }
        }

        public Spirdon(Build build, IRollLogger log, RollAnalyzer roller, HatysParty party) : base("Спиридон", build, log, roller, party)
        {
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

        public void CastVisageOfFenris()
        {
            if (!Self.CheckBonusExists(Build.Atributes.Manipulation, VizageOfFenris.GIFT_NAME))
            {
                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Intimidation, true, Verbosity.Details);
                //Cast Visage of fenfis
                var vizageOfFenris = new VizageOfFenris(Log, Roller);
                vizageOfFenris.Roll(Self, false, false);
            }
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

            var sacredFire = new SacredFire(Log, Roller, 0);
            sacredFire.Roll(Self, targets,  false, true);
        }

        public void WeeklyPreBoost(string suppTrait)
        {
            Log.Log(Verbosity.Details, "=== === === === ===");
            Log.Log(Verbosity.Details, string.Format("{0} WeeklyPreBoost on {1}", Self.Name, suppTrait));

            //-1 dc social rolls
            CastVisageOfFenris();
            //-1 dc social rolls
            CastPersuasion();
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);
            //apply
            CommonBuffs.ApplySacredRosemary(Party.Spiridon.Self, Log); //TODO: this usage should be counted once more because its target are spirits (do when we'll count talens)
            //buff support trait
            ApplyAncestors(suppTrait, Verbosity.Important);
        }

        public void WeeklyMidBoostOccult(Build target)
        {
            Log.Log(Verbosity.Details, "=== === === === ===");
            Log.Log(Verbosity.Details, string.Format("{0} WeeklyMidBoostOccult for {1}", Self.Name, target.Name));

            //for my next Ancestor Seeking
            CastSacredFire(new List<Build>() { Self });
            //cast rite of Anscestor Seeking - boost Occult to target
            CastAnscestorSeeking(target);
            //boost Occult to target
            CastCallToWyld(new List<Build>() { target }, Build.Abilities.Occult);
        }

        public void WeeklyBoostSkill(string mainTrait)
        {
            Log.Log(Verbosity.Details, "=== === === === ===");
            Log.Log(Verbosity.Details, string.Format("{0} WeeklyBoostSkill on {1}", Self.Name, mainTrait));

            //Buff occult from Spiridon
            Party.Spiridon.WeeklyMidBoostOccult(Self);

            //Maximum boost for trait
            CastGhostPack(mainTrait);
        }

        private void CastGhostPack(string trait)
        {
            //TODO: no check for multiple GhostPacks
            if (Self.AncestorsUsesLeft != -1)
            {
                Self.AncestorsUsesLeft += 1; //bonus usage

                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Occult, false, Verbosity.Details, true);

                if (BoneRhythmsUsagesLeft > 0)
                {
                    BoneRhythmsUsagesLeft--;
                    CommonBuffs.ApplyBoneRythms(Self, Log);
                }

                //roll Ghost Pack
                var ghostPackRoll = new GhostPack(Log, Roller);
                ghostPackRoll.Roll(Self, true, false);

                ApplyAncestors(trait, Verbosity.Critical);
            }
        }

        public void CastAnscestorSeeking(Build target)
        {
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);

            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Self, target, false, true);
        }

        public int CastMindPartition()
        {
            //Cast MindPartition
            var mindPartition = new MindPartition(Log, Roller);
            var extraActions = mindPartition.Roll(Self);
            WeeklyActions += extraActions;
            return extraActions;
        }

        public void __ActivateCarnyx(Build target, string purpose, bool withWill)
        {
            __ActivateCarnyx(new List<Build>() {target}, purpose, withWill);
        }

        public void __ActivateCarnyx(List<Build> targets, string purpose, bool withWill)
        {
            if (HasCarnyx)
            {
                Log.Log(Verbosity.Details, ">== Carnyx started, now actions from Spiridon");
                //TODO: -1 Gnosis to activate

                CastCaernChanneling(Build.Abilities.Performance);
                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Performance, false, Verbosity.Details, false); //NO RECURSION false

                if (targets.Contains(Self))
                {
                    throw new Exception("Spridon can't use Carnyx to buff himself");
                }

                var carnyx = new CarnyxOfVictory(Log, Roller, Verbosity.Details);
                carnyx.Roll(Self, targets, purpose, false, withWill);
            }
        }

        public void __DeactivateCarnyx()
        {
            if (HasCarnyx)
            {
                CarnyxOfVictory.RemoveFromBuild(Party.Builds.FindAll(build => !build.Name.Equals(CharacterName)));
                Log.Log(Verbosity.Details, "<== Carnyx ended, Spiridon can act again");
            }
        }

        public override bool HasSpecOnRite(Rite rite)
        {
            return rite.Info().Group == RiteGroup.Caern;
        }

        public override void CreateFetishBase(int fetishLevel, string fetishName)
        {
            CastCaernChanneling(Build.Abilities.Crafts);
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Crafts, false, Verbosity.Details);

            //Craft
            var craftFetishBase = new CraftFetishBase(Log, Roller);
            craftFetishBase.Roll(Self, fetishLevel, fetishName, false, true);


        }

        public override void CreateFetish(int fetishLevel, string fetishName, string spiritType)
        {
            CastCaernChanneling(Build.Abilities.Leadership);

            //TODO: Rite of Summoning, also Rite of ? to increase it's attitude (should be friendly for social talking)
            //TODO: Log roleplaying rites (Cleansing, Conrition, Chiminage, etc) - better to write GoogleDoc document
            /*
            CastSacredFire(new List<Build>() { Self });
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);
            var riteOfSummoning = new SummonSpirit(Log, Roller, new List<string>());
            riteOfSummoning.Roll(Self, spiritType, false, true);
            */

            //Persuade
            //TODO: add Chiminage
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Leadership, false, Verbosity.Details);
            var persuadeSpirit = new PesuadeSpiritEnterFetish(Log, Roller, Build.Atributes.Charisma, Build.Abilities.Leadership, new List<string>());
            persuadeSpirit.Roll(Self, fetishName, spiritType, false, true);

            //Rite of Fetish
            CastSacredFire(new List<Build>() { Self });
            Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);
            var riteOfFetish = new CreateFetishRite(Log, Roller, new List<string>());
            riteOfFetish.Roll(Self, fetishLevel, fetishName, false, true);
        }

        //TODO: Create Talens, Rite of Binding, Rite of Spirit Awakening, CreateCacao
        //!!!Create Talens WITH carnyx!!!

    }
}