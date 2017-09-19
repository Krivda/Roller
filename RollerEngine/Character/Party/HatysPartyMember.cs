using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;
using RollerEngine.Rolls.Backgrounds;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class HatysPartyMember : Common.Character
    {
        public const int SPEND_ALL_ATTEMPTS = -1;

        public HatysParty Party { get; private set; }
        public bool HasOpenedCaern { get; set; }
        public bool HasSpecOnInstruction { get; set; }

        public HatysPartyMember(string name, Build build, IRollLogger log, IRoller roller, HatysParty party) : base(name, build, log, roller)
        {
            Party = party;
            HasSpecOnInstruction = false;
        }

        public void ApplyAncestors(string trait)
        {
            CommonBuffs.ApplyAncestorsChiminage(Self, Log);
            var ansestorsRoll = new Ancestors(Log, Roller);
            ansestorsRoll.Roll(Self, trait);
        }

        public override void Learn(string ability, bool withWill)
        {
            List<TraitModifier> mods = Self.TraitModifiers.FindAll(m => m.Traits.Contains(ability));

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

                if (!mods.Exists(modifier => modifier.Name.Equals(Build.Backgrounds.Ancestors)))
                {
                    CommonBuffs.ApplyAncestorsChiminage(Self, Log);
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);

                    var ancestors = new Ancestors(Log, Roller);
                    ancestors.Roll(Self, ability);
                }
            }

            //Apply rosemary
            CommonBuffs.ApplySacredRosemary(Self, Log);

            if (!Self.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff Ability roll
                Party.Nameless.CastTeachersEase(Self, ability, false, Verbosity.Details);
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
                        xpPoolTraits.Add (new Tuple<string, int>(traitKvp.Key, traitKvp.Value));
                    }
                }
            }

            //TODO: sorting order?
            xpPoolTraits.Sort((tuple, tuple1) => tuple.Item2.CompareTo(tuple1.Item2));

            int spentAttempts = 0;

            foreach (var xpPoolTrait in xpPoolTraits)
            {
                string traitKeyXpPool = xpPoolTrait.Item1;
                string trait = Build.DynamicTraits.GetBaseTrait(traitKeyXpPool, Build.DynamicTraits.ExpiriencePool);
                bool hasWill = Self.Traits[trait] < 3;

                while (LearnAttempts > 0 )
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
                    LearnAttempts--;
                    spentAttempts++;

                }
            }
        }

        public virtual void Instruct(Build target, string ability, bool withWill)
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

                if (Self.Traits[Build.Backgrounds.Ancestors] > 0 && !Self.CheckBonusExists(Build.Abilities.Instruction, Build.Backgrounds.Ancestors))
                {
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Self, Log);
                    CommonBuffs.ApplyAncestorsChiminage(Self, Log);

                    ApplyAncestors(Build.Abilities.Instruction);
                }
                
                //give XP to smb
                var instruct = new InstructionTeach(Log, Roller);
                instruct.Roll(Self, target, ability, HasSpecOnInstruction, withWill);
            }
        }

        public void AutoLearnRite(int maxLearnAttempts)
        {
            var ritePoolTraits = new List<Tuple<string, int>>();

            //TODO: ???
            foreach (var traitKvp in Self.Traits)
            {
                if (traitKvp.Key.Contains(Build.DynamicTraits.RiteLearned))
                {
                    if (traitKvp.Value != Build.RiteAlreadyLearned)
                    {
                        ritePoolTraits.Add(new Tuple<string, int>(traitKvp.Key, traitKvp.Value));
                    }
                }
            }

            //TODO: sorting order?
            ritePoolTraits.Sort((tuple, tuple1) => tuple.Item2.CompareTo(tuple1.Item2));

            int spentAttempts = 0;

            foreach (var ritePoolTrait in ritePoolTraits)
            {
                string traitKeyRitePool = ritePoolTrait.Item1;
                string riteName = Build.DynamicTraits.GetBaseTrait(traitKeyRitePool, Build.DynamicTraits.RiteLearned);

                while (LearnAttempts > 0)
                {
                    //learn until don't exceed max attempts (or all available for SPEND_ALL_ATTEMPTS)
                    if ((maxLearnAttempts != SPEND_ALL_ATTEMPTS) && (spentAttempts == maxLearnAttempts))
                    {
                        break;
                    }

                    if (Self.Traits[traitKeyRitePool] == Build.RiteAlreadyLearned)
                    {
                        break;
                    }

                    LearnRite(riteName, false, true); //TODO check for Caern group for Spiridon, Mystic for Yoki etc
                                                      //always with Will to prevent botches (ask CURATOR!)
                    LearnAttempts--;
                    spentAttempts++;
                }
            }
        }

        public void LearnRite(string riteName, bool hasSpec, bool hasWill)
        {
            if (! (Self.CharacterClass.Equals(Build.Classes.Werewolf) || Self.CharacterClass.Equals(Build.Classes.Corax)))
            {
                throw new Exception(string.Format("{0} is {1}, and they can't learn rites", Self.Name, Self.CharacterClass));
            }

            //Apply rosemary
            CommonBuffs.ApplySacredRosemary(Self, Log);


            //ask nameless to buff instruct
            if (!Self.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff rituals
                Party.Nameless.CastTeachersEase(Self, Build.Abilities.Rituals, false, Verbosity.Details);
            }

            RitualsLearn roll = new RitualsLearn(Log, Roller);
            roll.Roll(Self, riteName, hasSpec, hasWill);
        }
    }
}