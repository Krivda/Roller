using System.Collections.Generic;
using RollerEngine.Character.Modifiers;
using RollerEngine.Logger;

namespace RollerEngine.Character
{
    public class CommonBuffs
    {
        public static void ApplyHatysBuff(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details , string.Format("Hatys totem power obtained by {0}", build.Name));

            build.DCModifiers.Add(
                new DCModifer(
                    "Hatys", 
                    new List<string>() {Build.Backgrounds.Ansestors},
                    DurationType.Scene,
                    new List<string>(),
                    -1
                ));
        }

        public static void ApplyAncestorsChiminage(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, string.Format("Ancestor spirits chiminage done by {0} (-1 DC on Ancestor Spirits interactions)", build.Name));

            build.BonusDCModifiers.Add(
                new DCModifer(
                    "Chiminage",
                    new List<string>() {},
                    DurationType.Roll,
                    new List<string>() {Build.Conditions.AncestorSpirits},
                    -1
                ));
        }


        public static void ApplyCaernOfVigilPower(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, string.Format("Caern of Vigil power applied on {0} (+4 Ancestors)", build.Name));

            build.TraitModifiers.Add(
                new TraitModifier(
                    "Caern of Vigil",
                    new List<string>() { Build.Backgrounds.Ansestors },
                    DurationType.Scene,
                    new List<string>(), 
                    4, 
                    TraitModifier.BonusTypeKind.AdditionalDice,
                    -1
                ));
        }

        public static void ApplyBoneRythms(Build build, IRollLogger log)
        {
            log.Log(Verbosity.Details, string.Format("Bone Rythms power applied on {0} (+1 Dice to next roll)", build.Name));

            build.BonusDicePoolModifiers.Add(
                new BonusModifier(
                    "Bone Rythmes",
                    DurationType.Roll,
                    new List<string>(),
                    1
                ));
        }

        public static void ApplyChannelling(Build build, IRollLogger log, int value)
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
    }
}