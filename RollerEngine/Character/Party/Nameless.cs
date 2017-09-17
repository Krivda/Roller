using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.Character.Party
{
    public class Nameless : HatysPartyMember
    {
        public Nameless(Build build, IRollLogger log, IRoller roller, HatysParty party) : base("Безымянный", build, log, roller, party)
        {
        }

        public void WeeklyPreBoost()
        {
            Log.Log(Verbosity.Important, "=== === === === ===");
            Log.Log(Verbosity.Important, string.Format("{0} WeeklyPreBoost", Self.Name));

            //-1 dc social rolls
            CastPersuasion();

            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);
        }

        public void WeeklyBoostSkill(string trait)
        {
            Log.Log(Verbosity.Important, "=== === === === ===");
            Log.Log(Verbosity.Important, string.Format("{0} WeeklyBoostSkill on {1}", Self.Name, trait));

            //buff Occult
            ApplyAncestors(Build.Abilities.Occult);

            //Buff occult from Spiridon
            Party.Spiridon.WeeklyMidBoostOccult(Self);

            //Maximum boost for trait
            CastGhostPack(trait, true);
        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Self, false, true);
        }

        public void CastTeachersEase(Build target, string ability, bool withWill, Verbosity verbosity)
        {
            //buff trait on target
            var teachersEase = new TeachersEase(Log, Roller, verbosity);
            teachersEase.Roll(Self, target, ability, true, withWill);
        }

        public static void ApplyChannellingGift(Build build, IRollLogger log, int value)
        {
            log.Log(Verbosity.Details, string.Format("{0} Channels {1} Rage to boost his next Action (+{1} Dice on next roll)", build.Name, value));

            build.BonusDicePoolModifiers.Add(
                new BonusModifier(
                    "Channeling",
                    DurationType.Roll,
                    new List<string>(),
                    value
                ));
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {

            //Cast persuasion before teaching
            CastPersuasion();

            base.Instruct(target, ability, withWill);
        }

        private void CastAnscestorSeeking(Build target)
        {
            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Self, target, false, true);
        }


        private void CastGhostPack(string trait, bool maxBoost)
        {
            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Self, Log);


            /*Self.DCModifiers.Add(
                new DCModifer(
                    "Dreams!",
                    new List<string>() { Build.Abilities.Occult },
                    DurationType.Roll,
                    new List<string>() { },
                    -2
                ));*/


            //roll Ghost Pack
            var ghostPackRoll = new GhostPack(Log, Roller);
            ghostPackRoll.Roll(Self, false, false);

            if (maxBoost)
            {
                //Apply Bone Ryhtms
                CommonBuffs.ApplyBoneRythms(Self, Log);

                //Apply Channelling for 3 Rage
                ApplyChannellingGift(Self, Log, 3);
            }

            ApplyAncestors(trait);

        }
    }
}