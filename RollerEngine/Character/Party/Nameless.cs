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

        public void WeeklyBoostSkill(string trait)
        {
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Build, Log);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Occult
            ApplyAncestors(Build.Abilities.Occult);

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

            //Apply Channelling for 3 Rage
            ApplyChannellingGift(Build, Log, 3);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Instruct 

            Build.UsedAncestorsCount = Build.UsedAncestorsCount - 1;
            ApplyAncestors(trait);

        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Build, false, true);
        }

        public void CastTeachersEase(Build target, string ability, bool withWill)
        {
            //buff trait on target
            var teachersEase = new TeachersEase(Log, Roller);
            teachersEase.Roll(Build, target, ability, true, withWill);
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

        private void CaseAnscestorSeeking()
        {
            //Cast Pesuasion
            var ancestorSeeking = new AncestorSeeking(Log, Roller);
            ancestorSeeking.Roll(Build, false, true);
        }

    }
}