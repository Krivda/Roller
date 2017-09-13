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
            List<TraitModifier> mods = Build.TraitModifiers.FindAll(m => m.Traits.Contains(ability));

            //Apply Caern Of Vigil Channelling and Ancestors (for warewolves only)
            if (Build.CharacterClass.Equals(Build.Classes.Warewolf))
            {
                //not already buffed with caern
                if (HasOpenedCaern && !mods.Exists(modifier => modifier.Name.Equals(CaernOfVigilChannelling.GiftName)))
                {
                    if (Party.CaernChannellingUsedTimes < 7)
                    {
                        if (!Build.Name.Equals(Party.Nameless.CharacterName))
                        {
                            //Ask Nameless to buff allertness
                            Party.Nameless.CastTeachersEase(Build, Build.Abilities.Alertness, false);
                        }

                        Party.CaernChannellingUsedTimes++;
                        var caernChanelling = new CaernOfVigilChannelling(Log, Roller);
                        caernChanelling.Roll(Build, ability, true);
                    }
                }

                if (!mods.Exists(modifier => modifier.Name.Equals(Build.Backgrounds.Ansestors)))
                {
                    CommonBuffs.ApplyAncestorsChiminage(Build, Log);
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Build, Log);

                    var ancestors = new Ancestors(Log, Roller);
                    ancestors.Roll(Build, ability);
                }
            }

            if (!Build.Name.Equals(Party.Nameless.CharacterName))
            {
                //Ask Nameless to buff Ability roll
                Party.Nameless.CastTeachersEase(Build, ability, false);
            }

            base.Learn(ability, withWill);
        }

        public void AutoLearn()
        {
            var xpPoolTraits = new List<Tuple<string, int>>();

            foreach (var traitKvp in Build.Traits)
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
                    bool hasWill = Build.Traits[trait] < 3;

                    
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
            if (Build.Traits[Build.Abilities.Instruction] > 0)
            {
                //ask nameless to buff instruct
                if (!Build.Name.Equals(Party.Nameless.CharacterName))
                {
                    //Ask Nameless to buff allertness
                    Party.Nameless.CastTeachersEase(Build, Build.Abilities.Instruction, false);
                }

                //Apply rosemary
                CommonBuffs.ApplySacredRosemary(Build, Log);

                if (Build.Traits[Build.Backgrounds.Ansestors] > 0 && !Build.CheckBonusExists(Build.Abilities.Instruction, Build.Backgrounds.Ansestors))
                {
                    CommonBuffs.ApplyCaernOfVigilPowerAncesctors(Build, Log);
                    CommonBuffs.ApplyAncestorsChiminage(Build, Log);

                    ApplyAncestors(Build.Abilities.Instruction);
                }
                
                //give XP to smb
                var instruct = new InstructionTeach(Log, Roller);
                instruct.Roll(Build, target, ability, HasSpecOnInstruction, withWill);
            }
        }

        public void ApplyAncestors(string trait)
        {
            var ansestorsRoll = new Ancestors(Log, Roller);
            ansestorsRoll.Roll(Build, trait);
        }

    }
}