using System.Collections.Generic;

namespace RollerEngine.Character.Common
{
    public partial class Build
    {
        public class CountersList
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

            public const int AmountAll = -1;

            public static string GetKey(string dynamicName, string baseName)
            {
                return string.Format("{0}{1}", dynamicName, baseName);
            }

            public static string GetBaseTrait(string key, string dynamicPrefix)
            {
                string result = key.Replace(dynamicPrefix, "").Trim();
                return result;
            }
        }


        public void InitCounters()
        {
            //spending Rage/Gnosis/Willpower
            Counters.Add(CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Rage), 0);
            Counters.Add(CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Gnosis), 0);
            Counters.Add(CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Willpower), 0);
/*
            //replenish Rage/Gnosis/Willpower
            Counters.Add(ReplenishPrefix + RollableTraits.Rage, 0);
            Counters.Add(ReplenishPrefix + RollableTraits.Gnosis, 0);
            Counters.Add(ReplenishPrefix + RollableTraits.Willpower, 0);
            //components from modules
            Counters.Add(ReplenishPrefix + ComponentPrefix, 21);           //TODO: WARNING DATE 05 Feb 2017
            Counters.Add(UsagePrefix + ComponentPrefix, 0);
            //sanctified
            Counters.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Rosemary], 0);
            Counters.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Rosemary], 0);
            Counters.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Tobacco], 0);
            Counters.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Tobacco], 0);
            Counters.Add(ReplenishPrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Cacao], 0);
            Counters.Add(UsagePrefix + SanctifiedPrefix + PlantsNames[SanctifiedPlants.Cacao], 0);
            //talens
            Counters.Add(ReplenishPrefix + TalenPrefix + TalenNames[Talens.Cacao], 0);
            Counters.Add(UsagePrefix + TalenPrefix + TalenNames[Talens.Cacao], 0);
            Counters.Add(ReplenishPrefix + TalenPrefix + TalenNames[Talens.GaiasBreath], 0);
            Counters.Add(UsagePrefix + TalenPrefix + TalenNames[Talens.GaiasBreath], 0);
            //spirits
            //dynamical
*/
        }

        public void CountersWeekStart()
        {
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Rage)] = 0;
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Gnosis)] = 0;
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Willpower)] = 0;
/*
            Counters[CountersList.ReplenishPrefix + RollableTraits.Rage] = 0;
            Counters[CountersList.ReplenishPrefix + RollableTraits.Gnosis] = 0;
            Counters[CountersList.ReplenishPrefix + RollableTraits.Willpower] = 0;
*/
        }

        public void SpendRage(int rageSpent)
        {
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Rage)] += rageSpent;
        }

        public void SpendGnosis(int gnosisSpent)
        {
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Gnosis)] += gnosisSpent;
        }

        public void SpendWillPower(int willSpent)
        {
            Counters[CountersList.GetKey(CountersList.SpentPrefix, RollableTraits.Willpower)] += willSpent;
        }

 /*
        public void ReplenishRage(int rageAmount)
        {
            if (rageAmount == CountersList.AmountAll) rageAmount = Traits[RollableTraits.Rage];
            Counters[CountersList.ReplenishPrefix + RollableTraits.Rage] += rageAmount;
        }

        public void ReplenishGnosis(int gnosisAmount)
        {
            if (gnosisAmount == CountersList.AmountAll) gnosisAmount = Traits[RollableTraits.Gnosis];
            Counters[CountersList.ReplenishPrefix + RollableTraits.Gnosis] += gnosisAmount;
        }

        public void ReplenishWillpower(int willAmount)
        {
            if (willAmount == CountersList.AmountAll) willAmount = Traits[RollableTraits.Willpower];
            Counters[CountersList.ReplenishPrefix + RollableTraits.Willpower] += willAmount;
        }

        public void AddModuleComponent(int no)
        {
            Counters[CountersList.ReplenishPrefix + CountersList.ComponentPrefix] += no;
        }

        public void UseModuleComponent(int no)
        {
            Counters[CountersList.UsagePrefix + CountersList.ComponentPrefix] += no;
        }

        public void AddSanctifiedPlant(CountersList.SanctifiedPlants plant, int no)
        {
            var key = CountersList.ReplenishPrefix + CountersList.SanctifiedPrefix + CountersList.PlantsNames[plant];
            Counters[key] += no;
        }

        public void SpendSanctifiedPlant(CountersList.SanctifiedPlants plant, int no)
        {
            var key = CountersList.UsagePrefix + CountersList.SanctifiedPrefix + CountersList.PlantsNames[plant];
            Counters[key] += no;
        }

        public void AddTalens(CountersList.Talens talen, int no)
        {
            var key = CountersList.ReplenishPrefix + CountersList.TalenPrefix + CountersList.TalenNames[talen];
            Counters[key] += no;
        }

        public void SpendTalens(CountersList.Talens talen, int no)
        {
            var key = CountersList.UsagePrefix + CountersList.TalenPrefix + CountersList.TalenNames[talen];
            Counters[key] += no;
        }

        public void AddSpirits(string spiritName, int no)
        {
            var key = CountersList.SpiritPrefix + spiritName;
            if (!Counters.ContainsKey(key))
            {
                Counters.Add(key, 0);
            }
            Counters[key] += no;
        }
*/

    }
}
