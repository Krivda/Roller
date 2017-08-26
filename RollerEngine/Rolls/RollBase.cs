﻿using System;
using System.Collections.Generic;
using System.Text;
using RollerEngine.Character;
using RollerEngine.Character.Modifiers;
using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Rolls
{
    public class RollBase
    {
        private const int BASE_DC = 6;
        private const int MIN_DC = 3;
        //private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IRollLogger _log;
        protected readonly IRoller _roller;

        public string Name { get; private set; }
        public List<string> DicePool { get; private set; }
        public bool RemoveSuccessesOn1 { get; private set; }
        public bool CanBotch { get; private set; }
        public List<string> Conditions { get; private set; }

        public class TraitValueInfo
        {
            public int BaseValue { get; set; }
            public int ModifiedValue { get; set; }
            public List<Tuple<int, TraitModifier>> Modifires { get; set; }
            public string TraitName { get; set; }
        }

        public class BonusValueInfo
        {
            public int Value { get; set; }
            public List<Tuple<int, BonusModifier>> Modifires { get; set; }
        }

        public class DicePoolInfo
        {
            public int Dices;
            public Dictionary<string, TraitValueInfo> Traits;
            public BonusValueInfo BonusDices;

        }

        public class TraitDCInfo
        {
            public int ModifiedValue { get; set; }
            public List<Tuple<int, DCModifer>> Modifires { get; set; }
            public string TraitName { get; set; }
        }

        public class BonusDCInfo
        {
            public int Value { get; set; }
            public List<Tuple<int, DCModifer>> Modifires { get; set; }
        }

        public class DCInfo
        {
            public int BaseDC;
            public int AdjustedDC;
            public Dictionary<string, TraitDCInfo> Traits;
            public BonusDCInfo BonusModifers;

        }

        public class RollInfo
        {
            public DicePoolInfo DicePoolInfo;
            public DCInfo DCInfo;
        }


        public RollBase(string name, IRollLogger log, IRoller roller,  List<String> dicePool, bool removeSuccessesOn1, bool canBotch, List<string> conditions)
        {
            _log = log;
            _roller = roller;
            Name = name;
            DicePool = dicePool;
            RemoveSuccessesOn1 = removeSuccessesOn1;
            CanBotch = canBotch;
            Conditions = conditions;
        }

        protected virtual int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            StringBuilder logMessageBefore = new StringBuilder(500);
            logMessageBefore.AppendLine();

            StringBuilder targetsStr = new StringBuilder(100);
            string delim = "";
            foreach (var target in targets)
            {
                targetsStr.Append(string.Format("{0}{1}", delim, target.Name));
                delim = ", ";
            }

            //Artze is rolling bless in Artze, Keltur with Specialization, with Will
            logMessageBefore.AppendFormat("{0} is rolling {1} on {2}", actor.Name, Name, targetsStr);

            if (hasSpec)
                logMessageBefore.Append(" with Specilization");
            if (hasWill)
                logMessageBefore.Append(" with Will");

            logMessageBefore.Append(".");

            _log.Log(Verbosity.Details, logMessageBefore.ToString());

            RollInfo info = GetRollInfo(actor, targets);

            string logMessage = GetLogForRoll(actor, targets, info, hasSpec, hasWill);
            _log.Log(Verbosity.Important, string.Format(logMessage));

            int successes = _roller.Roll(info.DicePoolInfo.Dices, info.DCInfo.AdjustedDC, RemoveSuccessesOn1, hasSpec, hasWill, Name);

            //remove used modifiers
            foreach (var traitValueInfo in info.DicePoolInfo.Traits)
            {
                foreach (var valueModifirer in traitValueInfo.Value.Modifires)
                {
                    if (valueModifirer.Item2.Duration == DurationType.Roll)
                    {
                        actor.TraitModifiers.Remove(valueModifirer.Item2);
                    }
                }
            }

            foreach (var bonusModifier in info.DicePoolInfo.BonusDices.Modifires)
            {
                    if (bonusModifier.Item2.Duration == DurationType.Roll)
                    {
                        actor.BonusDicePoolModifiers.Remove(bonusModifier.Item2);
                    }
            }

            foreach (var traitDCModifiers in info.DCInfo.Traits.Values)
            {
                foreach (var dcModifier in traitDCModifiers.Modifires)
                {
                    if (dcModifier.Item2.Duration == DurationType.Roll)
                    {
                        actor.DCModifiers.Remove(dcModifier.Item2);
                    }
                }
            }

            foreach (var bonusDCModifier in info.DCInfo.BonusModifers.Modifires)
            {
                if (bonusDCModifier.Item2.Duration == DurationType.Roll)
                {
                    actor.BonusDCModifiers.Remove(bonusDCModifier.Item2);
                }
            }

            if (CanBotch && successes < 0)
                throw new BotchException(string.Format("{0} roll botched on {1} successes.", Name, successes));

            return successes;
         }

        public RollInfo GetRollInfo(Build actor, List<Build> targets)
        {
            RollInfo rollInfo = new RollInfo();
            rollInfo.DicePoolInfo = GetDicePool(actor, targets);
            rollInfo.DCInfo = GetDCInfo(actor, targets);

            return rollInfo;
        }

        private DicePoolInfo GetDicePool(Build actor, List<Build> targets)
        {
            var dicePool = new DicePoolInfo();
            dicePool.Traits = new Dictionary<string, TraitValueInfo>();
            dicePool.Dices = 0;

            foreach (var trait in DicePool)
            {
                var traitInfo = GetModifiedTrait(actor, targets, trait, Conditions);
                dicePool.Traits.Add(trait, traitInfo);
                dicePool.Dices += traitInfo.ModifiedValue;
            }

            dicePool.BonusDices = GetBonusDices(actor, targets, Conditions);
            dicePool.Dices += dicePool.BonusDices.Value;

            return dicePool;
        }

        private TraitValueInfo GetModifiedTrait(Build actor, List<Build> targets, string traitName, List<string> conditions)
        {
            var appliedMods = new List<Tuple<int, TraitModifier>>();

            int baseValue;
            if (!actor.Traits.TryGetValue(traitName, out baseValue))
            {
                throw new Exception(string.Format("Trait {0} is not found", traitName));
            }
            int triatValue = baseValue;

            var modifiers = actor.TraitModifiers.FindAll(tm => tm.Traits.Contains(traitName));
            modifiers.Sort((tm1, tm2) => tm1.BonusType.CompareTo(tm2.BonusType));

            foreach (var modifier in modifiers)
            {
                if (!modifier.ConditionsMet(conditions))
                {
                    _log.Log(Verbosity.Debug, string.Format("Modifier {0} conditions aren't met.", modifier.Name));
                }
                else
                {
                    int modValue = modifier.Value;
                    //hadle limited value
                    int modValueLimitedValue = modifier.GetLimitedValue(actor, triatValue);

                    if (modValue == modValueLimitedValue)
                    {
                        triatValue += modifier.Value;
                    }
                    else
                    {
                        triatValue += modValueLimitedValue;
                        modValue = modValueLimitedValue;
                        _log.Log(Verbosity.Warning, string.Format("Modifer {0} is overcapped. Value set to {1}.", modifier.Name, modValueLimitedValue));
                    }

                    appliedMods.Add(new Tuple<int, TraitModifier>(modValue, modifier));
                }
            }

            return new TraitValueInfo() {TraitName = traitName, BaseValue = baseValue, ModifiedValue = triatValue, Modifires = appliedMods};
        }

        private BonusValueInfo GetBonusDices(Build actor, List<Build> targets, List<string> conditions)
        {
            var appliedMods = new List<Tuple<int, BonusModifier>>();
            int value = 0;

            var modifiers = actor.BonusDicePoolModifiers;

            foreach (var modifier in modifiers)
            {
                if (!modifier.ConditionsMet(conditions))
                {
                    _log.Log(Verbosity.Debug, string.Format("Modifier {0} conditions aren't met.", modifier.Name));
                }
                else
                {
                    //hadle limited value
                    value += modifier.Value;
                    appliedMods.Add(new Tuple<int, BonusModifier>(modifier.Value, modifier));
                }
            }

            return new BonusValueInfo() {Value = value, Modifires = appliedMods };
        }

        private DCInfo GetDCInfo(Build actor, List<Build> targets)
        {
            var dcInfo = new DCInfo();
            dcInfo.Traits = new Dictionary<string, TraitDCInfo>();
            dcInfo.BaseDC = GetBaseDC(actor, targets);
            int dcAdjust = 0;

            foreach (var trait in DicePool)
            {
                var traitInfo = GetTraitDCInfo(actor,targets, trait, Conditions);
                dcInfo.Traits.Add(trait, traitInfo);
                dcAdjust += traitInfo.ModifiedValue;
            }

            dcInfo.BonusModifers = GetBonusDC(actor, targets, Conditions);
            dcAdjust += dcInfo.BonusModifers.Value;
            int adjectedDC = dcInfo.BaseDC + dcAdjust;

            //hadle limited value
            if (adjectedDC < MIN_DC)
            {
                _log.Log(Verbosity.Warning, string.Format("DC was lesser then min, adjusted to ${0}.", MIN_DC));
                dcInfo.AdjustedDC = MIN_DC;
            }
            else
            {
                dcInfo.AdjustedDC = adjectedDC;
            }

            return dcInfo;
        }

        public virtual int GetBaseDC(Build actor, List<Build> targets)
        {
            return BASE_DC;
        }

        private TraitDCInfo GetTraitDCInfo(Build actor, List<Build> targets, string traitName, List<string> conditions)
        {
            var appliedMods = new List<Tuple<int, DCModifer>>();

            if (!actor.Traits.ContainsKey(traitName))
            {
                throw new Exception(string.Format("Trait {0} is not found.", traitName));
            }
            int adjustedValue = 0;

            var modifiers = actor.DCModifiers.FindAll(tm => tm.Traits.Contains(traitName));

            foreach (var modifier in modifiers)
            {
                if (!modifier.ConditionsMet(conditions))
                {
                    _log.Log(Verbosity.Debug, string.Format("Modifier {0} conditions aren't met.", modifier.Name));
                }
                else
                {
                    int modValue = modifier.Value;
                    //hadle limited value
                    appliedMods.Add(new Tuple<int, DCModifer>(modValue, modifier));
                    adjustedValue += modValue;
                }
            }

            return new TraitDCInfo() { TraitName = traitName, ModifiedValue = adjustedValue, Modifires = appliedMods };
        }

        private BonusDCInfo GetBonusDC(Build actor, List<Build> targets, List<string> conditions)
        {
            var appliedMods = new List<Tuple<int, DCModifer>>();

            int value = 0;

            var modifiers = actor.BonusDCModifiers;

            foreach (var modifier in modifiers)
            {
                if (!modifier.ConditionsMet(conditions))
                {
                    _log.Log(Verbosity.Debug, string.Format("Modifier {0} conditions aren't met.", modifier.Name));
                }
                else
                {
                    //hadle limited value
                    value += modifier.Value;
                    appliedMods.Add(new Tuple<int, DCModifer>(modifier.Value, modifier));
                }
            }

            return new BonusDCInfo() { Value = value, Modifires = appliedMods };
        }

        private string GetLogForRoll(Build actor, List<Build> targets, RollInfo info, bool hasSpec, bool hasWill)
        {
            StringBuilder logMessage = new StringBuilder(500);

            //Strength[8 +1(Crinos) +1(in Umbra)] + Brawl[4 +1(amultet)] +1(all rolls have +1) +4(Spirit Heritage) vs 6 [+1(in umbra) -1 (vs humans)]= 15d10vs6
            string delim = "";
            foreach (var traitInfo in info.DicePoolInfo.Traits.Values)
            {
                string traitModifiers = GetTraitModifiersLogMessage(traitInfo);

                logMessage.Append(string.Format("{0}{1}[{2}]", delim, traitInfo.TraitName, traitModifiers));
                delim = " + ";
            }

            foreach (var modifier in info.DicePoolInfo.BonusDices.Modifires)
            {
                logMessage.Append(string.Format(" {0:+0}({1})", modifier.Item2.Value, modifier.Item2.Name));
            }

            logMessage.Append(" vs ");

            string DCModifiers = GetDCModifiersLogMessage(info.DCInfo);
            logMessage.Append(DCModifiers);

            logMessage.Append(" = ");

            logMessage.Append(string.Format("{0}d10 vs {1}", info.DicePoolInfo.Dices, info.DCInfo.AdjustedDC));

            return logMessage.ToString();
        }

        private string GetDCModifiersLogMessage(DCInfo dcInfo)
        {
            StringBuilder message = new StringBuilder(100);


            List<ARollModifer> mods = new List<ARollModifer>();

            foreach (var trait in dcInfo.Traits.Values)
            {
                foreach (var traitMod in trait.Modifires)
                {
                    mods.Add(traitMod.Item2);
                }
            }

            foreach (var bonusMod in dcInfo.BonusModifers.Modifires)
            {
                mods.Add(bonusMod.Item2);
            }

            message.Append(string.Format("{0}", dcInfo.BaseDC));
            foreach (var modifier in mods)
            {
                message.Append(string.Format(" {0:+#;-#;0}({1})", modifier.Value, modifier.Name));
            }

            return message.ToString();
        }

        private string GetTraitModifiersLogMessage(TraitValueInfo traitInfo)
        {
            StringBuilder message = new StringBuilder(100);
            message.Append(string.Format("{0}", traitInfo.BaseValue));

            foreach (var modifier in traitInfo.Modifires)
            {
                message.Append(string.Format(" {0:+0}({1})", modifier.Item1, modifier.Item2.Name));
            }

            return message.ToString();
        }
    }

    public class BasicRoll : RollBase
    {
        public BasicRoll(string name, IRollLogger log, IRoller roller, List<string> dicePool, bool removeSuccessesOn1, bool canBotch, List<string> conditions) : base(name, log, roller, dicePool, removeSuccessesOn1, canBotch, conditions)
        {
        }

        public new int Roll(Build actor, List<Build> targets, bool hasSpec, bool hasWill)
        {
            return base.Roll(actor, targets, hasSpec, hasWill);
        }
    }
}
