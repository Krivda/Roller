using System;
using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.Rolls.Rites
{
    public class RiteRoll : RollBase
    {
        public Rite Rite { get; private set; }

        public RiteRoll(Rite rite, IRollLogger log, IRoller roller, List<string> dicePool, List<string> conditions, string addtionalInfo, Verbosity verbosity) :
            base(rite.Info().Name, log, roller, dicePool, true, true, conditions, addtionalInfo, verbosity)
        {
            Rite = rite;

            if (dicePool == null)
            {
                switch (Rite.Info().Group)
                {
                    case RiteGroup.Accord:
                    case RiteGroup.Death:
                    case RiteGroup.Punishment:
                    case RiteGroup.Renown:
                        DicePool = new List<string>() { Build.Atributes.Charisma, Build.Abilities.Rituals };
                        break;
                    case RiteGroup.Caern:
                        throw new Exception("Caern rites have no default dice pool");
                    case RiteGroup.Mystic:
                        DicePool = new List<string>() { Build.Atributes.Wits, Build.Abilities.Rituals };
                        break;
                    case RiteGroup.Seasonal:
                        DicePool = new List<string>() { Build.Atributes.Stamina, Build.Abilities.Rituals };
                        break;
                    case RiteGroup.Minor:
                        throw new Exception("Minor rites have no default dice pool");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            switch (Rite.Info().Group)
            {
                case RiteGroup.Accord:
                case RiteGroup.Caern:
                case RiteGroup.Mystic:
                case RiteGroup.Punishment:
                    return 7;
                case RiteGroup.Death:
                    throw new Exception("Death rites default DC is 8 - Rank");
                case RiteGroup.Renown:
                    return 6;
                case RiteGroup.Seasonal:
                    throw new Exception("Seasonal rites default DC is 8 - Caern Level");
                case RiteGroup.Minor:
                    throw new Exception("Minor rites have no default DC");
                default:
                    return base.GetBaseDC(actor, targets);
            }
        }

        protected override DicePoolInfo GetDicePool(Build actor, List<Build> targets)
        {
            var dicePool = base.GetDicePool(actor, targets);
            if (Rite.Info().Group == RiteGroup.Caern)
            {
                var gnosisValue = actor.Traits[Build.RollableTraits.Gnosis];
                //truncate to Gnosis value
                if (dicePool.Dices > gnosisValue)
                {
                    dicePool = new DicePoolInfo
                    {
                        Dices = gnosisValue,
                        Traits = new Dictionary<string, TraitValueInfo>()
                        {
                            {
                                Build.RollableTraits.Gnosis,
                                new TraitValueInfo
                                {
                                    BaseValue = gnosisValue,
                                    ModifiedValue = gnosisValue,
                                    Modifires = new List<Tuple<int, TraitModifier>>(),
                                    TraitName = Build.RollableTraits.Gnosis
                                }
                            }
                        },
                        BonusDices = new BonusValueInfo() {Value = 0, Modifires = new List<Tuple<int, BonusModifier>>() }
                    };
                    return dicePool;
                }
            }
            return dicePool;
        }

    }
}
