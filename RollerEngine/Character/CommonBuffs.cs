using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;

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
                    new List<string>() { Build.Backgrounds.Ansestors },
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


        public static void ApplyCaernOfVigilPowerAncesctors(Build build, IRollLogger log)
        {
            const string caernOfVigil = "Caern of Vigil";

            log.Log(Verbosity.Details, string.Format("{0} power applied on {1} (+4 Ancestors)", caernOfVigil, build.Name));

            if (!build.TraitModifiers.Exists(m => m.Name.Equals(caernOfVigil)))
            {
                build.TraitModifiers.Add(
                    new TraitModifier(
                        caernOfVigil,
                        new List<string>() {Build.Backgrounds.Ansestors},
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
            log.Log(Verbosity.Details, string.Format("Bone Rythms power applied on {0} (+1 Dice to next roll)", build.Name));

            build.BonusDicePoolModifiers.Add(
                new BonusModifier(
                    "Bone Rythmes",
                    DurationType.Roll,
                    new List<string>(),
                    1
                ));
        }
    }
}