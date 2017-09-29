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

        public void WeeklyPreBoost(NamelessBuff buffPlan)
        {
            Log.Log(Verbosity.Details, ActivityChannel.Boost, "=== === === === ===");
            Log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} WeeklyPreBoost for {1}", Self.Name, buffPlan.PreBuff.Trait));

            //-1 dc social rolls
            CastPersuasion();

            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);

            if (buffPlan.PreBuff.UseBoneRhythms)
            {
                CommonBuffs.ApplyBoneRythms(Self, Log);
            }

            if (buffPlan.PreBuff.UseRageChanneling)
            {
                ApplyChannellingGift(Self, Log, 3);
            }

            //buff support trait
            if (!string.IsNullOrEmpty(buffPlan.PreBuff.Trait))
            {
                ApplyAncestors(buffPlan.PreBuff.Trait, Verbosity.Important);
            }
        }

        public void WeeklyBoostSkill(NamelessBuff buff)
        {
            Log.Log(Verbosity.Details, ActivityChannel.Boost, "=== === === === ===");
            Log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} WeeklyBoostSkill on {1}", Self.Name, buff.MainBuff.Trait));

            //Buff occult from Spiridon
            Party.Spiridon.WeeklyMidBoostOccult(Self);

            //Maximum boost for trait
            //Party.Spiridon.ActivateCarnyx();
            CastGhostPack(buff.MainBuff);
            //Party.Spiridon.DeactivateCarnyx();
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
            log.Log(Verbosity.Details, ActivityChannel.Boost, string.Format("{0} Channels {1} Rage to boost his next Action (+{1} Dice on next roll)", build.Name, value));

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

        private void CastGhostPack(NamelessBuff.ApplyBuffs buff)
        {
            //TODO: no check for multiple GhostPacks
            if (Self.AncestorsUsesLeft != -1)
            {
                Self.AncestorsUsesLeft += 1; //bonus usage

                //Apply chiminage
                CommonBuffs.ApplyAncestorsChiminage(Self, Log);

                //TODO: Spirit Lore
                /*Self.DCModifiers.Add(
                    new DCModifer(
                        "Spirit Lore Dreams!",
                        new List<string>() { Build.Abilities.Occult },
                        DurationType.Roll,
                        new List<string>() { },
                        -1
                    ));*/

                //roll Ghost Pack
                var ghostPackRoll = new GhostPack(Log, Roller);
                ghostPackRoll.Roll(Self, false, false);

                if (buff.UseBoneRhythms)
                {
                    //Apply Bone Ryhtms
                    CommonBuffs.ApplyBoneRythms(Self, Log);
                }
                if (buff.UseRageChanneling)
                {
                    //Apply Channelling for 3 Rage
                    ApplyChannellingGift(Self, Log, 3);
                }

                ApplyAncestors(buff.Trait, Verbosity.Critical);
            }

        }
    }
}