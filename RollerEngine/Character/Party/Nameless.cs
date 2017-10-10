using System.Activities.Statements;
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

        public bool HasUnbrokenCord
        {
            get
            {
                return Self.Items.ContainsKey("Unbroken Cord"); 
            }
        }

        public bool HasVisageOfFenris
        {
            get
            {
                if (Self.Traits.ContainsKey(Build.Abilities.VisageOfFenris))
                {
                    //we need 6XP to learn Visage og Fenris gift, this is almost the same as 7XP needed to learn 2 dots in ability
                    if (Self.Traits[Build.Abilities.VisageOfFenris] >= 2)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public NamelessBuff BoostPlan { get; set; }

        public Nameless(Build build, IRollLogger log, IRollAnalyzer roller, HatysParty party) : base("Безымянный", build, log, roller, party)
        {
        }

        public void WeeklyPreBoost(NamelessBuff buffPlan)
        {
            Log.Log(Verbosity.Details, "=== === === === ===");
            Log.Log(Verbosity.Details, string.Format("{0} WeeklyPreBoost for {1}", Self.Name, buffPlan.PreBuff.Trait));

            Party.Spiridon.__ActivateCarnyx(Self, "fast pre-boost", true);
            //-1 dc social rolls
            CastPersuasion();
            //-1 dc social rolls
            CastVisageOfFenris();
            Party.Spiridon.__DeactivateCarnyx();

            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);

            if (buffPlan.PreBuff.UseBoneRhythms)
            {
                if (BoneRhythmsUsagesLeft > 0)
                {
                    BoneRhythmsUsagesLeft--;
                    CommonBuffs.ApplyBoneRythms(Self, Log);
                }
            }

            if (buffPlan.PreBuff.UseRageChanneling)
            {
                ApplyChannellingGift(Self, Log, 3);
            }

            //buff support trait
            if (!string.IsNullOrEmpty(buffPlan.PreBuff.Trait))
            {
                ApplyAncestors(buffPlan.PreBuff.Trait, Verbosity.Details);
            }
        }

        public void WeeklyBoostSkill(NamelessBuff buff)
        {
            Log.Log(Verbosity.Details, "=== === === === ===");
            Log.Log(Verbosity.Details, string.Format("{0} WeeklyBoostSkill on {1}", Self.Name, buff.MainBuff.Trait));

            //Buff occult from Spiridon
            Party.Spiridon.WeeklyMidBoostOccult(Self);

            //Maximum boost for trait
            CastGhostPack(buff.MainBuff);
        }

        public void CastPersuasion()
        {
            //Cast Pesuasion
            CastCaernChanneling(Build.Abilities.Subterfuge);
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Self, false, true);
        }

        public void CastTeachersEase(Build target, string ability, bool withWill, Verbosity verbosity, bool carnyxed = false)
        {
            //buff trait on target
            var teachersEase = new TeachersEase(Log, Roller, verbosity);
            if (carnyxed)
            {
                Party.Spiridon.__ActivateCarnyx(Self, "Teacher's ease", true);
            }
            teachersEase.Roll(Self, target, ability, true, withWill);
            if (carnyxed)
            {
                Party.Spiridon.__DeactivateCarnyx();
            }
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
            CastVisageOfFenris();
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

                bool namelessHasSpec = Self.Traits[Build.Abilities.Occult] > 3;
                Party.Spiridon.__ActivateCarnyx(Self, "Main boost Nameless", false);
                ghostPackRoll.Roll(Self, namelessHasSpec, false);
                Party.Spiridon.__DeactivateCarnyx();

                if (buff.UseBoneRhythms)
                {
                    if (BoneRhythmsUsagesLeft > 0)
                    {
                        BoneRhythmsUsagesLeft--;
                        CommonBuffs.ApplyBoneRythms(Self, Log);
                    }
                }
                if (buff.UseRageChanneling)
                {
                    //Apply Channelling for 3 Rage
                    ApplyChannellingGift(Self, Log, 3);
                }

                CastCaernChanneling(Build.Abilities.Occult);

                //remove prev bonus to same trait from Ancestors
                var traitMod = Self.TraitModifiers.Find(tm => tm.Traits.Contains(buff.Trait) && tm.Name.Equals(Build.Backgrounds.Ancestors));
                if (traitMod != null)
                {
                    Self.TraitModifiers.Remove(traitMod);
                }

                ApplyAncestors(buff.Trait, Verbosity.Important);
            }
        }

        public void CastVisageOfFenris()
        {
            if (HasVisageOfFenris)
            {
                //Cast Visage of fenfis
                CastCaernChanneling(Build.Abilities.Intimidation);
                var vizageOfFenris = new VizageOfFenris(Log, Roller);
                vizageOfFenris.Roll(Self, false, false);
            }
        }
    }
}