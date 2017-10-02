using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Backgrounds;
using RollerEngine.Rolls.Rites;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class HatysPartyMember : Common.Character
    {
        public const int SPEND_ALL_ATTEMPTS = -1;

        public HatysParty Party { get; private set; }
        public bool HasOpenedCaern { get; set; }
        public bool HasSpecOnInstruction { get; set; }

        public int BoneRhythmsUsagesLeft;

        public HatysPartyMember(string name, Build build, IRollLogger log, IRoller roller, HatysParty party) : base(
            name, build, log, roller)
        {
            Party = party;
            HasSpecOnInstruction = false;
        }

        public void ApplyAncestors(string trait, Verbosity verbosity)
        {
            CommonBuffs.ApplyAncestorsChiminage(Self, Log);
            var ansestorsRoll = new Ancestors(Log, Roller, verbosity);
            ansestorsRoll.Roll(Self, trait);
        }

        public override void Learn(string ability, bool withWill)
        {
            //Apply Caern Of Vigil Channelling and Ancestors (for warewolves only)
            if (Self.CharacterClass.Equals(Build.Classes.Werewolf))
            {
                //not already buffed with caern
                /*if (HasOpenedCaern && !mods.Exists(modifier => modifier.Name.Equals(CaernOfVigilChannelling.GiftName)))
                {
                    if (Party.CaernChannellingUsedTimes < 7)
                    {
                        if (!Self.Name.Equals(Party.Nameless.CharacterName))
                        {
                            //Ask Nameless to buff allertness
                            Party.Nameless.CastTeachersEase(Self, Self.Abilities.Alertness, false, Verbosity.Details);
                        }

                        Party.CaernChannellingUsedTimes++;
                        var caernChanelling = new CaernOfVigilChannelling(Log, Roller);
                        caernChanelling.Roll(Self, ability, true);
                    }
                }*/

                if (Self.AncestorsUsesLeft > 0)
                {
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);

                    ApplyAncestors(ability, Verbosity.Important);
                }
            }

            //Apply rosemary
            CommonBuffs.ApplySacredRosemary(Self, Log);

            if (!Self.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff Ability roll
                Party.Nameless.CastTeachersEase(Self, ability, false, Verbosity.Details);
            }

            //Apply Bone Rhythms
            if (BoneRhythmsUsagesLeft > 0)
            {
                BoneRhythmsUsagesLeft--;
                CommonBuffs.ApplyBoneRythms(Self, Log);
            }

            base.Learn(ability, withWill);
        }

        public void AutoLearn(int maxLearnAttempts)
        {
            var xpPoolTraits = new List<Tuple<string, int>>();

            foreach (var traitKvp in Self.Traits)
            {
                if (traitKvp.Key.Contains(Build.DynamicTraits.ExpiriencePool))
                {
                    if (traitKvp.Value != 0)
                    {
                        xpPoolTraits.Add(new Tuple<string, int>(traitKvp.Key, traitKvp.Value));
                    }
                }
            }

            //TODO: THIS IS WRONG!! NEED TO DEBUG
            xpPoolTraits.Sort((tuple, tuple1) => tuple.Item2.CompareTo(tuple1.Item2));

            int spentAttempts = 0;

            foreach (var xpPoolTrait in xpPoolTraits)
            {
                string traitKeyXpPool = xpPoolTrait.Item1;
                string trait = Build.DynamicTraits.GetBaseTrait(traitKeyXpPool, Build.DynamicTraits.ExpiriencePool);
                bool hasWill = Self.Traits[trait] < 4;

                while (WeeklyPartialActions > 0)
                {
                    //learn until don't exceed max attempts (or all available for SPEND_ALL_ATTEMPTS)
                    if ((maxLearnAttempts != SPEND_ALL_ATTEMPTS) && (spentAttempts == maxLearnAttempts))
                    {
                        break;
                    }

                    if (Self.Traits[traitKeyXpPool] <= 0) //TODO: AlreadyLearned
                    {
                        break;
                    }

                    Learn(trait, hasWill);
                    WeeklyPartialActions--;
                    spentAttempts++;
                }
            }
        }

        public override void Instruct(Build target, string ability, bool withWill)
        {
            if (Self.Traits[Build.Abilities.Instruction] > 0)
            {
                //ask nameless to buff instruct
                if (!Self.Name.Equals(Party.Nameless.CharacterName))
                {
                    //Ask Nameless to buff allertness
                    Party.Nameless.CastTeachersEase(Self, Build.Abilities.Instruction, false, Verbosity.Details);
                }

                //Apply rosemary
                CommonBuffs.ApplySacredRosemary(Self, Log);

                if (Self.AncestorsUsesLeft > 0)
                {
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);

                    ApplyAncestors(Build.Abilities.Instruction, Verbosity.Important);
                }

                //give XP to smb
                var instruct = new InstructionTeach(Log, Roller);
                instruct.Roll(Self, target, ability, HasSpecOnInstruction, withWill);
            }
        }

        public void AutoLearnRite(int maxLearnAttempts)
        {
            var ritePoolTraits = new List<Tuple<RiteInfo, int>>();

            foreach (var traitKvp in Self.Traits)
            {
                if (traitKvp.Key.Contains(Build.DynamicTraits.RiteLearned))
                {
                    if (traitKvp.Value != Build.RiteAlreadyLearned)
                    {
                        string riteName =
                            Build.DynamicTraits.GetBaseTrait(traitKvp.Key, Build.DynamicTraits.RiteLearned);
                        RiteInfo rinfo = RitesDictionary.Rites.First(ri => ri.Value.Name.Equals(riteName)).Value;
                        ritePoolTraits.Add(new Tuple<RiteInfo, int>(rinfo, traitKvp.Value));
                    }
                }
            }

            //sort by number of success left to learn the rite
            ritePoolTraits.Sort((tuple, tuple1) =>
                ((int)(tuple .Item1.Level * 10) - tuple .Item2).CompareTo(
                 (int)(tuple1.Item1.Level * 10) - tuple1.Item2));


            int spentAttempts = 0;

            foreach (var ritePoolTrait in ritePoolTraits)
            {
                string riteName = ritePoolTrait.Item1.Name;
                Rite rite = ritePoolTrait.Item1.Rite;

                while (WeeklyPartialActions > 0)
                {
                    //learn until don't exceed max attempts (or all available for SPEND_ALL_ATTEMPTS)
                    if ((maxLearnAttempts != SPEND_ALL_ATTEMPTS) && (spentAttempts == maxLearnAttempts))
                    {
                        break;
                    }

                    if (Self.IsRiteLearned(riteName))
                    {
                        break;
                    }

                    LearnRite(rite, false); //TODO check for Caern group for Spiridon, Mystic for Yoki etc
                    //always with Will to prevent botches (ask CURATOR!)
                    WeeklyPartialActions--;
                    spentAttempts++;
                }
            }
        }

        public void LearnRite(Rite rite, bool hasWill)
        {
            if (!(Self.CharacterClass.Equals(Build.Classes.Werewolf) ||
                  Self.CharacterClass.Equals(Build.Classes.Corax)))
            {
                throw new Exception(string.Format("{0} is {1}, and they can't learn rites", Self.Name,
                    Self.CharacterClass));
            }
            RiteInfo riteInfo;
            if (!RitesDictionary.Rites.TryGetValue(rite, out riteInfo))
            {
                throw new Exception(string.Format("Rite {0} is not known by software!", Enum.GetName(typeof(Rite), rite)));
            }

            var riteName = riteInfo.Name;

            //Apply rosemary
            CommonBuffs.ApplySacredRosemary(Self, Log);
            
            //ask nameless to buff instruct
            if (!Self.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff rituals
                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);
            }

            RitualsLearn roll = new RitualsLearn(Log, Roller);
            roll.Roll(Self, rite, HasSpecOnRite(rite), hasWill);
        }

        public virtual bool HasSpecOnRite(Rite rite)
        {
            return false;
        }

        public virtual void CreateFetishBase(int fetishLevel, string fetishName)
        {
            throw new Exception("Abstact method, override in actual party member");
        }

        public virtual void CreateFetish(int fetishLevel, string fetishName, string spiritType)
        {
            throw new Exception("Abstact method, override in actual party member");
        }

    }
}