using System;
using System.Collections.Generic;
using System.Linq;

namespace RollerEngine.WodSystem.WTA
{
    public enum RiteGroup
    {
        Accord,
        Caern,
        Death,
        Mystic,
        Punishment,
        Renown,
        Seasonal,
        Minor
    }

    public enum Rite
    {
        //Core:
        Cleansing,              //Accord, 1             //roleplay; manual rolling              //NOCODE: we all know this rite => no need to spend Gnosis
        Contrition,             //Accord, 1             //roleplay; manual rolling              //NOCODE: due to Sacred Fire this roll will be automatic success
        Renunciation,           //Accord, 2             //roleplay; manual rolling
        MootRite,               //Caern, 1              //roleplay; manual rolling
        OpenedCaern,            //Caern, 1              //todo: need to code
        BadgersBurrow,          //Caern, 4              //roleplay; manual rolling; NOTOKEN
        OpenedBridge,           //Caern, 4              //roleplay; manual rolling; NOTOKEN
        ShroudedGlen,           //Caern, 4              //roleplay; manual rolling; NOTOKEN     DEFENSE
        CaernBuilding,          //Caern, 5              //roleplay; manual rolling
        GatheringForDeparted,   //Death, 1              //replaced by Rememverance
        WinterWolf,             //Death, 3              //roleplay; manual rolling; NOTOKEN
        BaptismOfFire,          //Mystic, 1             //roleplay; manual rolling
        Binding,                //Mystic, 1             //todo: need to code
        QuestingStone,          //Mystic, 1             //roleplay; manual rolling              INVESTIGATION
        TalismanDedication,     //Mystic, 1             //roleplay; manual rolling
        Becoming,               //Mystic, 2             //roleplay; manual rolling
        SpiritAwakening,        //Mystic, 2             //todo: need to code
        Summoning,              //Mystic, 2             //todo: need to code
        Fetish,                 //Mystic, 3             //todo CODED
        Totem,                  //Mystic, 3             //roleplay; manual rolling; NOTOKEN
        Ostracism,              //Punishment, 1         //roleplay; manual rolling
        StoneOfScorn,           //Punishment, 2         //roleplay; manual rolling
        VoiceOfJackal,          //Punishment, 2         //roleplay; manual rolling
        //Punishment rites 3+ are skipped
        Accomplishment,         //Renown, 2             //roleplay; manual rolling
        Passage,                //Renown, 2             //roleplay; manual rolling
        Wounding,               //Renown, 1             //roleplay; manual rolling
        //Seasonal rites were skipped
        BoneRhythms,            //Minor                 //todo CODED; NOROLL
        BreathOfGaia,           //Minor                 //roleplay; manual                   TODO learn all in Arc7
        GreetMoon,              //Minor                 //roleplay; manual                   TODO learn all in Arc7
        GreetSun,               //Minor                 //roleplay; manual                   TODO learn all in Arc7
        HuntingPrayer,          //Minor                 //roleplay; manual                   TODO learn all in Arc7
        PrayerForPrey,          //Minor                 //roleplay; manual                   TODO learn all in Arc7

        //PGtG (selected only)
        OpenedSky,              //Accord, 4             //roleplay; manual                      SUPER-CLEANSING
        Adoration,              //Caern, 2              //roleplay; manual
        LesserMourning,         //Death, 2              //roleplay; manual
        PreservingFetish,       //Mystic, 1             //roleplay; manual                      ONCE PER MONTH; TODO maybe code later
        RenewingTalen,          //Mystic, 2             //todo need to CODE usages
        Praise,                 //Renown, 2             //roleplay; manual

        //Bone Gnawers (selected only)
        CrashSpace,             //Mystic, 2             //todo: need to code                    REPLENISH
        ShoppingCart,           //Mystic, 2             //roleplay; manual
        Signpost,               //Caern, 4              //roleplay; manual                      DEFENSE
        Trepassing,             //Caern, 5              //roleplay; manual                      DEFENSE

        //Children of Gaia (selected only)
        Teachers,               //Minor                 //roleplay; manual
        BowelsOfMother,         //Minor                 //roleplay; manual
        Comfort,                //Accord, 2             //roleplay; manual
        Askllepios,             //Mystic, 3             //roleplay; manual
        SinEatingGaia,          //Mystic, 3             //roleplay; manual
        TalismanAdaptation,     //Mystic, 3             //roleplay; manual
        SacredPeace,            //Caern, 5              //roleplay; manual                      DEFENSE
        PartedVeil,             //Mystic, 5             //roleplay; manual

        //Uktena (selected only)
        Balance,                //Accord, 3             //roleplay; manual
        SacredFire,             //Mystic, 1             //todo CODED
        PrayerOfSeeking,        //Mystic, 1             //roleplay; manual
        SpiritCage,             //Mystic, 3             //roleplay; manual
        InvitationToAncestors,  //Mystic, 4             //roleplay; manual

        //Wendigo (selected only)
        Rememberance,           //Death, 1              //roleplay; manual
        SinEaterWendigo,        //Death, 2              //roleplay; manual
        Nightshade,             //Death, 4              //roleplay; manual
        SunDance,               //Mystic, 2             //roleplay; manual
        Deliverance,            //Mystic, 3             //roleplay; manual

        //Stargazers (selected only)
        VigorOfDeparted,        //Death, 2              //roleplay; manual
        Knowing,                //Mystic, 2             //todo need to CODE; Enigmas; replenish gnosis
        TeaCeremony,            //Minor, 0              //roleplay; manual
        AncestorVeneration,     //Minor, 0              //todo CODED; NOROLL

        //Misc
        FeastForSpirits,        //Mystic, 2 //Fianna                    //AUTOMATIC SUCCESS
        Heritage,               //Mystic, 1 //Get of Fenris             //roleplay; manual
        HonorableOath,          //Accord, 1 //Silverfangs               //roleplay; manual
        AncestorSeeking         //Mystic, 1 //Spiridon's research       //todo CODED
    }

    public static class RiteExtensions
    {
        public static RiteInfo Info(this Rite rite)
        {
            return RiteInfo.RitesDictionary.Rites[rite];
        }

        public static int SuccessesRequiredToLearn(this RiteInfo riteInfo)
        {
            if (riteInfo.Level == 0) return 5;
            return 10 * riteInfo.Level;
        }
    }

    public class RiteInfo
    {
        public Rite Rite { get; private set; }
        public string Name { get; private set; }
        public RiteGroup Group { get; private set; }
        public int Level { get; private set; }
        public List<string> Conditions { get; private set; }

        private RiteInfo(string riteName, RiteGroup riteGroup, int riteLevel, List<string> conditions)
        {
            Name = riteName;
            Group = riteGroup;
            Level = riteLevel;
            Conditions = conditions;
        }

        private void Init(Rite rite)
        {
            Rite = rite;
        }

        public static RiteInfo ByName(string riteName)
        {
            return RitesDictionary.Rites.First(ri => ri.Value.Name.Equals(riteName)).Value;
        }

        public static class RitesDictionary
        {
            public static Dictionary<Rite, RiteInfo> Rites { get; private set; }

            static RitesDictionary()
            {
                Rites = new Dictionary<Rite, RiteInfo>
                {
                    {Rite.Cleansing, new RiteInfo("Of Cleansing", RiteGroup.Accord, 1, new List<string>())},
                    {Rite.Contrition, new RiteInfo("Of Contrition", RiteGroup.Accord, 1, new List<string>())},
                    {Rite.Renunciation, new RiteInfo("Of Renunciation", RiteGroup.Accord, 2, new List<string>())},

                    {Rite.MootRite, new RiteInfo("Of Moot", RiteGroup.Caern, 1, new List<string>())},
                    {Rite.OpenedCaern, new RiteInfo("Of Opened Caern", RiteGroup.Caern, 1, new List<string>())},
                    {Rite.BadgersBurrow, new RiteInfo("Of Badger`s Burrow", RiteGroup.Caern, 4, new List<string>())},
                    {Rite.OpenedBridge, new RiteInfo("Of Opened Bridge", RiteGroup.Caern, 4, new List<string>())},
                    {Rite.ShroudedGlen, new RiteInfo("Of Shrouded Glen", RiteGroup.Caern, 4, new List<string>())},
                    {Rite.CaernBuilding, new RiteInfo("Of Caern Building", RiteGroup.Caern, 5, new List<string>())},
                    {Rite.GatheringForDeparted, new RiteInfo("Of Gathering For Departed", RiteGroup.Death, 1, new List<string>())},
                    {Rite.WinterWolf, new RiteInfo("Of Winter Wolf", RiteGroup.Death, 3, new List<string>())},

                    {Rite.BaptismOfFire, new RiteInfo("Baptism Of Fire", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.Binding, new RiteInfo("Of Binding", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.QuestingStone, new RiteInfo("Of Questing Stone", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.TalismanDedication, new RiteInfo("Of Talisman Dedication", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.Becoming, new RiteInfo("Of Becoming", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.SpiritAwakening, new RiteInfo("Of Spirit Awakening", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.Summoning, new RiteInfo("Of Summoning", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.Fetish, new RiteInfo("Of Fetish", RiteGroup.Mystic, 3, new List<string>())},
                    {Rite.Totem, new RiteInfo("Of Totem", RiteGroup.Mystic, 3, new List<string>())},

                    {Rite.Ostracism, new RiteInfo("Of Ostracism", RiteGroup.Punishment, 1, new List<string>())},
                    {Rite.StoneOfScorn, new RiteInfo("Stone Of Scorn", RiteGroup.Punishment, 2, new List<string>())},
                    {Rite.VoiceOfJackal, new RiteInfo("Voice Of Jackal", RiteGroup.Punishment, 2, new List<string>())},

                    {Rite.Accomplishment, new RiteInfo("Of Accomplishment", RiteGroup.Renown, 2, new List<string>())},
                    {Rite.Passage, new RiteInfo("Of Passage", RiteGroup.Renown, 2, new List<string>())},
                    {Rite.Wounding, new RiteInfo("Of Wounding", RiteGroup.Renown, 1, new List<string>())},

                    {Rite.BoneRhythms, new RiteInfo("Of Bone Rhythms", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.BreathOfGaia, new RiteInfo("Breath Of Gaia", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.GreetMoon, new RiteInfo("Of Greet the Moon", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.GreetSun, new RiteInfo("Of Greet the Sun", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.HuntingPrayer, new RiteInfo("Of Hunting Prayer", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.PrayerForPrey, new RiteInfo("Of Prayer For the Prey", RiteGroup.Minor, 0, new List<string>())},

                    {Rite.OpenedSky, new RiteInfo("Of Opened Sky", RiteGroup.Accord, 4, new List<string>())},
                    {Rite.Adoration, new RiteInfo("Of Adoration", RiteGroup.Caern, 2, new List<string>())},
                    {Rite.LesserMourning, new RiteInfo("Of Lesser Mourning", RiteGroup.Death, 2, new List<string>())},
                    {Rite.PreservingFetish, new RiteInfo("Of Preserving Fetish", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.RenewingTalen, new RiteInfo("Of Renewing Talen", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.Praise, new RiteInfo("Of Praise", RiteGroup.Renown, 2, new List<string>())},

                    {Rite.CrashSpace, new RiteInfo("Of Crash Space", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.ShoppingCart, new RiteInfo("Of Shopping Cart", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.Signpost, new RiteInfo("Of Signpost", RiteGroup.Caern, 4, new List<string>())},
                    {Rite.Trepassing, new RiteInfo("Of Trepassing", RiteGroup.Caern, 5, new List<string>())},

                    {Rite.Teachers, new RiteInfo("Of the Teachers", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.BowelsOfMother, new RiteInfo("Bowels Of Mother", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.Comfort, new RiteInfo("Of Comfort", RiteGroup.Accord, 2, new List<string>())},
                    {Rite.Askllepios, new RiteInfo("Of Askllepios", RiteGroup.Mystic, 3, new List<string>())},
                    {Rite.SinEatingGaia, new RiteInfo("Of Sin-Eating (Gaia)", RiteGroup.Mystic, 3, new List<string>())},
                    {Rite.TalismanAdaptation, new RiteInfo("Of Talisman Adaptation", RiteGroup.Mystic, 3, new List<string>())},
                    {Rite.PartedVeil, new RiteInfo("Of Parted Veil", RiteGroup.Mystic, 5, new List<string>())},
                    {Rite.SacredPeace, new RiteInfo("Of Sacred Peace", RiteGroup.Caern, 5, new List<string>())},

                    {Rite.Balance, new RiteInfo("Of Balance", RiteGroup.Accord, 1, new List<string>())},
                    {Rite.SacredFire, new RiteInfo("Of Sacred Fire", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.PrayerOfSeeking, new RiteInfo("Prayer Of the Seeking", RiteGroup.Mystic, 1, new List<string>())},
                    {Rite.SpiritCage, new RiteInfo("Of Spirit Cage", RiteGroup.Mystic, 3, new List<string>())},
                    {Rite.InvitationToAncestors, new RiteInfo("Of Invitation To Ancestors", RiteGroup.Mystic, 4, new List<string>())},

                    {Rite.Rememberance, new RiteInfo("Of Rememberance", RiteGroup.Death, 1, new List<string>())},
                    {Rite.SinEaterWendigo, new RiteInfo("Of Sin-Eater (Wendigo)", RiteGroup.Death, 2, new List<string>())},
                    {Rite.Nightshade, new RiteInfo("Of Nightshade", RiteGroup.Death, 4, new List<string>())},
                    {Rite.SunDance, new RiteInfo("Of Sun Dance", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.Deliverance, new RiteInfo("Of Deliverance", RiteGroup.Mystic, 3, new List<string>())},

                    {Rite.VigorOfDeparted, new RiteInfo("Vigor Of Departed", RiteGroup.Death, 2, new List<string>())},
                    {Rite.Knowing, new RiteInfo("Of Knowing", RiteGroup.Mystic, 2, new List<string>())},
                    {Rite.TeaCeremony, new RiteInfo("Tea Ceremony", RiteGroup.Minor, 0, new List<string>())},
                    {Rite.AncestorVeneration, new RiteInfo("Of Ancestor Veneration", RiteGroup.Minor, 0, new List<string>())},

                    {Rite.FeastForSpirits, new RiteInfo("Feast For the Spirits", RiteGroup.Mystic, 2, new List<string>())}, //Fianna
                    {Rite.Heritage, new RiteInfo("Of Heritage", RiteGroup.Mystic, 1, new List<string>())},                  //Get of Fenris
                    {Rite.HonorableOath, new RiteInfo("Of Honorable Oath", RiteGroup.Accord, 1, new List<string>())},       //Sliverfangs
                    {Rite.AncestorSeeking, new RiteInfo("Of Ancestor Seeking", RiteGroup.Mystic, 1, new List<string>())}    //Spiridon's research
                };

                foreach (var rite in Rites)
                {
                    rite.Value.Init(rite.Key);
                }
            }
        }
    }
}

