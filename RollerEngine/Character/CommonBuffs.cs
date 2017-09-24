using System.Collections.Generic;
using NLog.Config;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;

namespace RollerEngine.Character
{
    public class CommonBuffs
    {
        public static void ApplyHatysBuff(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("Hatys totem power obtained by {0}", build.Name));

            build.DCModifiers.Add(
                new DCModifer(
                    "Hatys",
                    new List<string>() { Build.Backgrounds.Ancestors },
                    DurationType.Permanent,
                    new List<string>(),
                    -2
                ));

            build.DCModifiers.Add(
                new DCModifer(
                    "Hatys",
                    new List<string>() { Build.Abilities.Rituals },
                    DurationType.Permanent,
                    new List<string>() {Build.Conditions.LearningRites},
                    -1
                ));
        }

        public static void ApplyAncestorsChiminage(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("Ancestor spirits chiminage done by {0} (-1 DC on Ancestor Spirits interactions)", build.Name));

            build.BonusDCModifiers.Add(
                new DCModifer(
                    "Chiminage",
                    new List<string>() {},
                    DurationType.Roll,
                    new List<string>() {Build.Conditions.AncestorSpirits},
                    -1
                ));
        }

        public static void ApplyCaernOfVigilPowerAncesctors(Build build, IRollLogger log)
        {
            const string caernOfVigil = "Caern of Vigil";

            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("{0} power applied on {1} (+4 Ancestors)", caernOfVigil, build.Name));

            if (!build.TraitModifiers.Exists(m => m.Name.Equals(caernOfVigil)))
            {
                build.TraitModifiers.Add(
                    new TraitModifier(
                        caernOfVigil,
                        new List<string>() {Build.Backgrounds.Ancestors},
                        DurationType.Scene,
                        new List<string>(),
                        4,
                        TraitModifier.BonusTypeKind.AdditionalDice,
                        -1
                    ));
            }
        }

        public static void ApplyBoneRythms(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("Bone Rythms power applied on {0} (+1 Dice to next roll)", build.Name));

            build.BonusDicePoolModifiers.Add(
                new BonusModifier(
                    "Bone Rythmes",
                    DurationType.Roll,
                    new List<string>(),
                    1
                ));
        }

        public static void ApplySacredRosemary(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("Rosemary sanctified plant power applied on {0} (-2 Dice on learning to next roll)", build.Name));

            if (!build.CheckBonusExists(null, "Rosemary"))
            {
                build.BonusDCModifiers.Add(
                    new DCModifer(
                        "Rosemary",
                        new List<string>(),
                        DurationType.Scene,
                        new List<string>()
                        {
                            Build.Conditions.Teaching,
                            Build.Conditions.Learning,
                            Build.Conditions.Memory,
                            Build.Conditions.LearningRites
                        },
                        -2
                    ));
            }
        }

        //TODO: ApplyCacao

        public static void ApplyMedicalBundle(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("Medical Bundle power applied on {0} (+3 Dice on Stamina rols)", build.Name));

            build.TraitModifiers.Add(
                new TraitModifier(
                    "Medical Bundle",
                    new List<string>() {Build.Atributes.Stamina},
                    DurationType.Permanent,
                    new List<string>(),
                    3,
                    TraitModifier.BonusTypeKind.AdditionalDice
                ));

            build.BonusDCModifiers.Add(
                new DCModifer(
                    "Medical Bundle",
                    new List<string>(),
                    DurationType.Permanent,
                    new List<string>() {Build.Conditions.AncestorSocial},
                    -1
                ));

        }

        public static void ShiftToCrinos(Build actor, IRollLogger log)
        {
            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("{0} shifted to Crinos (+4 Strength, +1 Dex, +3 Stamina)", actor.Name));

            actor.TraitModifiers.Add(
                new TraitModifier(
                    "Crinos",
                    new List<string>() { Build.Atributes.Strength },
                    DurationType.Scene,
                    new List<string>(),
                    4,
                    TraitModifier.BonusTypeKind.TraitMod
                ));

            actor.TraitModifiers.Add(
                new TraitModifier(
                    "Crinos",
                    new List<string>() { Build.Atributes.Dexterity },
                    DurationType.Scene,
                    new List<string>(),
                    1,
                    TraitModifier.BonusTypeKind.TraitMod
                ));

            actor.TraitModifiers.Add(
                new TraitModifier(
                    "Crinos",
                    new List<string>() { Build.Atributes.Stamina },
                    DurationType.Scene,
                    new List<string>(),
                    3,
                    TraitModifier.BonusTypeKind.TraitMod
                ));

        }

        public static void ApplyHeightenSenses(Build actor, IRollLogger log)
        {
            string name = "Heighten senses";

            log.Log(Verbosity.Details, ActivityChannel.Intermediate, string.Format("{0} applied {1} (-3 DC Peception, +1 dice to Primal Urge)", actor.Name, name));

            actor.DCModifiers.Add(
                new DCModifer(
                    "Heighten senses",
                    new List<string>() {Build.Atributes.Perception},
                    DurationType.Roll,
                    new List<string>() {},
                    -3
                ));

            actor.TraitModifiers.Add(
                new TraitModifier(
                    name,
                    new List<string>() { Build.Abilities.PrimalUrge },
                    DurationType.Scene,
                    new List<string>(),
                    1,
                    TraitModifier.BonusTypeKind.AdditionalDice
                ));
        }
    }
}