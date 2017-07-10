using System;
using System.Collections.Generic;
using RollerEngine.Character.Modifiers;

namespace RollerEngine.Character
{
    public class Build
    {
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
        }

        public class Backgrounds
        {
            public const string Ansestors = "Ansestors";
            public const string SpiritHeritage = "Spirit Heritage";
        }

        public class RollableTraits
        {
            public const string Will = "Will";
            public const string Gnosis = "Gnosis";
            public const string Rage = "Rage";
        }

        public Dictionary<string, int> Traits = new Dictionary<string, int>();
        
        public List<TraitModifier> TraitModifiers { get; set; } = new List<TraitModifier>();
        public List<DCModifer> DCModifiers { get; set; } = new List<DCModifer>();
        public List<BonusModifier> BonusDicePoolModifiers { get; set; } = new List<BonusModifier>();

        public List<DCModifer> BonusDCModifiers { get; set; } = new List<DCModifer>();
        public object Name { get; set; }

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
                Traits.Add(field.Name, 0);
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
    }
}
