using System.Collections.Generic;

namespace RollerEngine.Character.Common
{
    public partial class Build
    {
        public class Counters
        {
            public enum SanctifiedPlants
            {
                Rosemary,       //-2 diff for lerning/teaching/memory   (WW)
                Tobacco,        //for Sacred Fire                       (Uktena)
                Cacao,          //for Talens                            (Mokole)
                //Aloe,         //                                      (?)

                //Basil,        //                                      (SH)
                //Catmint,      //for use with Lynx                     (SH)
                //Foxglove,     //+2 diff for faerie magic              (SH)
                //Peyote,       //                                      (SH)
                //Plaintain,    //                                      (SH)
                //Yaupon        //                                      (SH)
            }

            public enum Talens
            {
                Cacao,                                                  //(Mokole)
                GaiasBreath                                             //(H&K)
                //SunArrow                                              //(Uktena)
                //Bloody Bondages                                       //(H&K)
            }

            public static readonly Dictionary<SanctifiedPlants, string> PlantsNames = new Dictionary<SanctifiedPlants, string>
            {
                {SanctifiedPlants.Rosemary, "Rosemary"},
                {SanctifiedPlants.Tobacco, "Tobacco"},
                {SanctifiedPlants.Cacao, "Cacao"}
            };

            public static readonly Dictionary<Talens, string> TalenNames = new Dictionary<Talens, string>()
            {
                {Talens.Cacao, "Cacao"},
                {Talens.GaiasBreath, "Gaia's Breath"}
            };

            public const string SpentPrefix = "Has spent ";
            public const string UsagePrefix = "Usages of ";
            public const string ReplenishPrefix = "Replenish of ";
            public const string ComponentPrefix = "Components";          //from modules
            public const string SanctifiedPrefix = "Sanctified ";
            public const string TalenPrefix = "Talen ";
            public const string SpiritPrefix = "Spirit of";

            public Dictionary<string, int> Dictionary = new Dictionary<string, int>();

            public void InitCounters()
            {
                //spending Rage/Gnosis/Willpower
                Dictionary.Add(SpentPrefix + RollableTraits.Rage, 0);
                Dictionary.Add(SpentPrefix + RollableTraits.Gnosis, 0);
                Dictionary.Add(SpentPrefix + RollableTraits.Willpower, 0);
                //replenish Rage/Gnosis/Willpower
                Dictionary.Add(ReplenishPrefix + RollableTraits.Rage, 0);
                Dictionary.Add(ReplenishPrefix + RollableTraits.Gnosis, 0);
                Dictionary.Add(ReplenishPrefix + RollableTraits.Willpower, 0);
                //components from modules
                Dictionary.Add(ReplenishPrefix + ComponentPrefix, 21);           //TODO: WARNING DATE 05 Feb 2017
                Dictionary.Add(UsagePrefix + ComponentPrefix, 0);
                //sanctified
                Dictionary.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Rosemary], 0);
                Dictionary.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Rosemary], 0);
                Dictionary.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Tobacco], 0);
                Dictionary.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Tobacco], 0);
                Dictionary.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Cacao], 0);
                Dictionary.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Cacao], 0);
                //talens
                Dictionary.Add(ReplenishPrefix + TalenPrefix + TalenNames[Talens.Cacao], 0);
                Dictionary.Add(UsagePrefix + TalenPrefix + TalenNames[Talens.Cacao], 0);
                Dictionary.Add(ReplenishPrefix + TalenPrefix + TalenNames[Talens.GaiasBreath], 0);
                Dictionary.Add(UsagePrefix + TalenPrefix + TalenNames[Talens.GaiasBreath], 0);
                //spirits
                //dynamical
            }

            public const int AmountAll = -1;
        }

        private readonly Counters _counters = new Counters();

        public void SpendRage(int rageSpent)
        {
            _counters.Dictionary[Counters.SpentPrefix + RollableTraits.Rage] += rageSpent;
        }

        public void SpendGnosis(int gnosisSpent)
        {
            _counters.Dictionary[Counters.SpentPrefix + RollableTraits.Gnosis] += gnosisSpent;
        }

        public void SpendWillPower(int willSpent)
        {
            _counters.Dictionary[Counters.SpentPrefix + RollableTraits.Willpower] += willSpent;
        }

        public void ReplenishRage(int rageAmount)
        {
            if (rageAmount == Counters.AmountAll) rageAmount = Traits[RollableTraits.Rage];
            _counters.Dictionary[Counters.ReplenishPrefix + RollableTraits.Rage] += rageAmount;
        }

        public void ReplenishGnosis(int gnosisAmount)
        {
            if (gnosisAmount == Counters.AmountAll) gnosisAmount = Traits[RollableTraits.Gnosis];
            _counters.Dictionary[Counters.ReplenishPrefix + RollableTraits.Gnosis] += gnosisAmount;
        }

        public void ReplenishWillpower(int willAmount)
        {
            if (willAmount == Counters.AmountAll) willAmount = Traits[RollableTraits.Willpower];
            _counters.Dictionary[Counters.ReplenishPrefix + RollableTraits.Willpower] += willAmount;
        }

        public void AddModuleComponent(int no)
        {
            _counters.Dictionary[Counters.ReplenishPrefix + Counters.ComponentPrefix] += no;
        }

        public void UseModuleComponent(int no)
        {
            _counters.Dictionary[Counters.UsagePrefix + Counters.ComponentPrefix] += no;
        }

        public void AddSanctifiedPlant(Counters.SanctifiedPlants plant, int no)
        {
            var key = Counters.ReplenishPrefix + Counters.SanctifiedPrefix + Counters.PlantsNames[plant];
            _counters.Dictionary[key] += no;
        }

        public void SpendSanctifiedPlant(Counters.SanctifiedPlants plant, int no)
        {
            var key = Counters.UsagePrefix + Counters.SanctifiedPrefix + Counters.PlantsNames[plant];
            _counters.Dictionary[key] += no;
        }

        public void AddTalens(Counters.Talens talen, int no)
        {
            var key = Counters.ReplenishPrefix + Counters.TalenPrefix + Counters.TalenNames[talen];
            _counters.Dictionary[key] += no;
        }

        public void SpendTalens(Counters.Talens talen, int no)
        {
            var key = Counters.UsagePrefix + Counters.TalenPrefix + Counters.TalenNames[talen];
            _counters.Dictionary[key] += no;
        }

        public void AddSpirits(string spiritName, int no)
        {
            var key = Counters.SpiritPrefix + spiritName;
            if (!_counters.Dictionary.ContainsKey(key))
            {
                _counters.Dictionary.Add(key, 0);
            }
            _counters.Dictionary[key] += no;
        }
    }
}
