using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;
using RollerEngine.Rolls.Backgrounds;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class HatysPartyMember : Common.Character
    {
        public HatysParty Party { get; private set; }
        public bool HasOpenedCaern { get; set; }
        public bool HasSpecOnInstruction { get; set; }

        public HatysPartyMember(string name, Build build, IRollLogger log, IRoller roller, HatysParty party) : base(name, build, log, roller)
        {
            Party = party;
            HasSpecOnInstruction = false;
            LearnSessions = 1;
        }

        public override void Learn(string ability, bool withWill)
        {
            List<TraitModifier> mods = Self.TraitModifiers.FindAll(m => m.Traits.Contains(ability));

            //Apply Caern Of Vigil Channelling and Ancestors (for warewolves only)
            if (Self.CharacterClass.Equals(Build.Classes.Warewolf))
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

            if (!Self.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff Ability roll
                Party.Nameless.CastTeachersEase(Self, ability, false, Verbosity.Important);
            }

            base.Learn(ability, withWill);
        }

        public void AutoLearn()
        {
            var xpPoolTraits = new List<Tuple<string, int>>();

            foreach (var traitKvp in Self.Traits)
            {
                if (traitKvp.Key.Contains(Build.DynamicTraits.ExpirienceToLearn))
                {
                    if (traitKvp.Value != 0)
                    {
                        xpPoolTraits.Add (new Tuple<string, int>(traitKvp.Key, traitKvp.Value));
                    }
                }
            }

            xpPoolTraits.Sort((tuple, tuple1) => tuple.Item2.CompareTo(tuple1.Item2));

            foreach (var xpPoolTrait in xpPoolTraits)
            {
                if (LearnSessions > 0)
                {
                    string trait = xpPoolTrait.Item1.Replace(Build.DynamicTraits.ExpirienceToLearn, "").Trim();
                    bool hasWill = Self.Traits[trait] < 3;

                    
                    for (int i = 0; i < LearnSessions; i++)
                    {
                        Learn(trait, hasWill);
                    }
                    LearnSessions=0;

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
                    Party.Nameless.CastTeachersEase(Self, Build.Abilities.Instruction, false, Verbosity.Important);
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

        public void ApplyAncestors(string trait)
        {
            CommonBuffs.ApplyAncestorsChiminage(Self, Log);
            var ansestorsRoll = new Ancestors(Log, Roller);
            ansestorsRoll.Roll(Self, trait);
        }

        public void LearnRite(string riteName, int riteLevel, bool hasSpec)
        {
            if (! (Self.CharacterClass.Equals(Build.Classes.Warewolf) || Self.CharacterClass.Equals(Build.Classes.Corax)))
            {
                throw new Exception(string.Format("{0} is {1}, and they can't learn rites", Self.Name, Self.CharacterClass));
            }
            
        }
    }
}