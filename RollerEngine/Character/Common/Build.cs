using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Modifiers;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.Character.Common
{
    public class Build
    {
        public class Classes
        {
            public const string Werewolf = "Werewolf";
            public const string Kinfolk = "Kinfolk";
            public const string Corax = "Corax";
        }
        
        public class Atributes
        {
            public const string Strength = "Strength";
            public const string Dexterity = "Dexterity";
            public const string Stamina = "Stamina";

            public const string Charisma = "Charisma";
            public const string Manipulation = "Manipulation";
            public const string Appearance = "Appearance";

            public const string Perception = "Perception";
            public const string Intellect  = "Intellect";
            public const string Wits       = "Wits";
        }
        
        //TODO we should split traits into Atributes, Abilities, Backgrounds, Rollalbe-10-based, Dynamic
        public class Abilities
        {
            //talents
            public const string Alertness = "Alertness";
            public const string Athletics = "Athletics";
            public const string Brawl = "Brawl";
            public const string Dodge = "Dodge";
            public const string Empathy = "Empathy";
            public const string Expression = "Expression";
            public const string Intimidation = "Intimidation";
            public const string PrimalUrge = "Primal Urge";
            public const string Streetwise = "Streetwise";
            public const string Subterfuge = "Subterfuge";
            public const string Instruction = "Instruction";
            
            //skills
            public const string AnimalKen = "Animal Ken";
            public const string Crafts = "Crafts";
            public const string Drive = "Drive";
            public const string Pilot = "Pilot";
            public const string Etiquette = "Etiquette";
            public const string Firearms = "Firearms";
            public const string Leadership = "Leadership";
            public const string Melee = "Melee";
            public const string Performance = "Performance";
            public const string Stealth = "Stealth";
            public const string Survival = "Survival";
            public const string Archery = "Archery";
            public const string Demolitions = "Demolitions";
            public const string KlaiveDueling = "Klaive Dueling";
            public const string Meditation = "Meditation";
            public const string Traps = "Traps";
            public const string Stargazing = "Stargazing";
            public const string Hypnotism = "Hypnotism";
            public const string Repair = "Repair";

            //Knowledges
            public const string Bureaucracy = "Bureaucracy";
            public const string Computers = "Computers";
            public const string Enigmas = "Enigmas";
            public const string Investigation = "Investigation";
            public const string Law = "Law";
            public const string Linguistics = "Linguistics";
            public const string Medicine = "Medicine";
            public const string Occult = "Occult";
            public const string Politics = "Politics";
            public const string Rituals = "Rituals";
            public const string Science = "Science";
            public const string AreaKnowledge = "Area Knowledge";
            public const string Cosmology = "Cosmology";
            public const string Herbalism = "Herbalism";
            public const string Poison = "Poison";
            public const string WyrmLore = "Wyrm Lore";
            public const string SpiritLore = "Spirit Lore";

            //strange
            public const string Mimicry = "Mimicry";
            public const string Search = "Search";
            public const string Swimming = "Swimming";
            public const string Ventriloquism = "Ventriloquism";
            public const string Gambling = "Gambling";
            public const string Kailindo = "Kailindo";
            public const string Wrestling = "Wrestling";
            public const string Gesture = "Gesture";
            public const string Stickball = "Stickball";
            public const string Iskakku = "Iskakku";
            public const string FaerieLore = "Faerie Lore";
            public const string TribalLore = "Tribal Lore";
            public const string AnimalLore = "Animal Lore";

            //todo: PIZDA. A bad-ass hack
            public const string VisageOfFenris = "Visage Of Fenris"; 
        }

        public class Backgrounds
        {
            public const string Ancestors = "Ancestors";
            public const string SpiritHeritage = "Spirit Heritage";

        }

        public class RollableTraits
        {
            public const string Willpower = "Willpower";
            public const string Gnosis = "Gnosis";
            public const string Rage = "Rage";
        }

        public class Conditions
        {
            public const string AncestorSpirits = "Ancestor Spirits";
            public const string SpiritHeritage = "Spirit Heritage";
            public const string Social = "Social";
            public const string Teaching = "TeachAbility";
            public const string Learning = "Learning";
            public const string MysticRite = "Mysctic Rite";
            public const string SpiritRite = "Spirit Rite";
            public const string GaianSpiritsSocial = "Gaian spirits Social";
            public const string Memory = "Memory";
            public const string LearningRites = "Learning Rites";

            //for ignored
            public const string Background = "Background";
            public const string RollableTrait = "RollableTrait";
        }

        public class DynamicTraits
        {
            public const string ExpiriencePool = "Expirience Pool";
            public const string ExpirienceLearned = "Expirience Learned";
            public const string RitePool = "Rite Pool";
            public const string RiteLearned = "Rite Learned";

            public static string GetKey(string dynamicName, string baseName)
            {
                return string.Format("{0} {1}", dynamicName, baseName);
            }

            public static string GetBaseTrait(string key, string dynamicPrefix)
            {
                string result = key.Replace(dynamicPrefix, "").Trim();
                return result;
            }
        }

        public int AncestorsUsesLeft { get; set; }

        public string CharacterClass { get; set; }

        public Dictionary<string, int> Traits = new Dictionary<string, int>();
                                                                     //active count
        public Dictionary<string, int> Items = new Dictionary<string, int>();
                                                                     //active from week
        public Dictionary<Rite, int> Rites = new Dictionary<Rite, int>();

        public List<TraitModifier> TraitModifiers = new List<TraitModifier>();
        public List<DCModifer> DCModifiers = new List<DCModifer>();
        public List<BonusModifier> BonusDicePoolModifiers = new List<BonusModifier>();

        public static Dictionary<int, int> GetSkillXpTable()
        {
            Dictionary<int, int> result = new Dictionary<int, int>
            {
                {0, 0}, {1, 3}, {2, 4}, {3, 6}, {4, 8}, {5, 10}
            };

            //Exp
            //1   3   3
            //2   4   7
            //3   6   13
            //4   8   21
            //5   10  31


            return result;
        }

        public List<DCModifer> BonusDCModifiers = new List<DCModifer>();
        public string Name { get; set; }
        public bool HasAncestorVeneration { get; set; }

        public Build(string name)
        {
            Name = name;
            InitTraits();
        }

        private void InitTraits()
        {

            AddTraits(typeof(Abilities));
            AddTraits(typeof(Atributes));
            AddTraits(typeof(Backgrounds));
            AddTraits(typeof(RollableTraits));
        }

        protected void AddTraits(Type clazz)
        {
            foreach (var field in clazz.GetFields())
            {
                Traits.Add(field.GetValue(null).ToString(), 0);
            }
        }

        public void AddTraitModifer(TraitModifier modifer)
        {
            TraitModifiers.Add(modifer);
        }

        public void AddDCModifer(DCModifer modifer)
        {
            DCModifiers.Add(modifer);
        }

        public void AddDicePoolBonusModifer(BonusModifier modifier)
        {
            BonusDicePoolModifiers.Add(modifier);
        }

        public bool CheckBonusExists(string trait, string bonusName)
        {
            if (! string.IsNullOrEmpty(trait))
            {
                if (TraitModifiers.Any(m => m.Name.Equals(bonusName)))
                {
                    return true;
                }

                if (DCModifiers.Any(m => m.Name.Equals(bonusName)))
                {
                    return true;
                }
            }
            else
            {
                if (BonusDCModifiers.Any(m => m.Name.Equals(bonusName)))
                {
                    return true;
                }

                if (BonusDicePoolModifiers.Any(m => m.Name.Equals(bonusName)))
                {
                    return true;
                }
            }

            return false;
        }

        public int GetModifiedTrait(string traitName)
        {
            var mods = TraitModifiers.FindAll(m => m.Traits.Contains(traitName) &&
                                                   (m.BonusType == TraitModifier.BonusTypeKind.TraitMod ||
                                                    m.BonusType == TraitModifier.BonusTypeKind.TraitModLimited));

            mods.Sort((tm1, tm2) => tm1.BonusType.CompareTo(tm2.BonusType));


            int traitValue = Traits[traitName];

            foreach (var traitModifier in mods.FindAll(m => m.BonusType == TraitModifier.BonusTypeKind.TraitMod))
            {
                traitValue += traitModifier.Value;
            }

            foreach (var traitModifier in mods.FindAll(m => m.BonusType == TraitModifier.BonusTypeKind.TraitModLimited))
            {
                traitValue += traitModifier.GetLimitedValue(this, traitValue);
            }

            return traitValue;
        }

        public const int RiteAlreadyLearned = -1;

        public bool IsRiteLearned(string riteName)
        {
            string keyRiteName = DynamicTraits.GetKey(DynamicTraits.RiteLearned, riteName);

            //create dynamic trait if it was absent
            if (!Traits.ContainsKey(keyRiteName))
            {
                return false;
            }

            int successesInitial = Traits[keyRiteName];
            return (successesInitial == RiteAlreadyLearned);
        }
    }
}
