using System.Collections.Generic;
using NUnit.Framework;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;
using RollerEngine.Rolls;

namespace UnitTests
{
    
    public class BuildTest
    {
        [Test]
        public void RollSimpleTest()
        {
            var roller = new OfflineDiceRoller(new StringBufferLogger());
            IRollLogger logger = new StringBufferLogger();

            Build build = new Build("Artze");
            int dex = 4;
            int brawl = 3;

            build.Traits[Build.Atributes.Dexterity] = dex;
            build.Traits[Build.Abilities.Brawl] = brawl;

            string name = "Attack!";
            var targets = new List<Build> {build};
            var dicepool = new List<string> {Build.Atributes.Dexterity, Build.Abilities.Brawl};
            var conditions = new List<string>();
            bool hasSpec = true;
            bool hasWill = true;
            bool remove1 = true;

            BasicRoll roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>() );
            var info =  roll.GetRollInfo(build, new List<Build> {build});

            Assert.AreEqual(dex+brawl, info.DicePoolInfo.Dices, "dice pool");

            Assert.AreEqual(dex, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");

            Assert.AreEqual(6, info.DCInfo.AdjustedDC, "adjusted DC");
            Assert.AreEqual(6, info.DCInfo.BaseDC, "base DC");

            roll.Roll(build, targets, hasSpec, hasWill);
        }


        [Test(Description = "Trait modifiers")]
        public void RollTestTraitMods()
        {
            var roller = new OfflineDiceRoller(new StringBufferLogger());
            IRollLogger logger = new StringBufferLogger();

            Build build = new Build("Artze");
            int dex = 3;
            int brawl = 3;

            build.Traits[Build.Atributes.Dexterity] = dex;
            build.Traits[Build.Abilities.Brawl] = brawl;

            string name = "Attack!";
            var targets = new List<Build> { build };
            var dicepool = new List<string> { Build.Atributes.Dexterity, Build.Abilities.Brawl };
            var conditions = new List<string>();
            bool hasSpec = true;
            bool hasWill = true;
            bool remove1 = true;

            var crinos = new TraitModifier("crinos", new List<string> { Build.Atributes.Dexterity }, DurationType.Scene, conditions, 1, TraitModifier.BonusTypeKind.TraitMod);
            build.AddTraitModifer(crinos);

            BasicRoll roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>());
            var info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            Assert.AreEqual(dex + brawl +1, info.DicePoolInfo.Dices, "dice pool");

            Assert.AreEqual(dex +1, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");

            var amultetDex = new TraitModifier("amulet Dex", new List<string> { Build.Atributes.Dexterity }, DurationType.Scene, conditions, 2, TraitModifier.BonusTypeKind.TraitModLimited, 5);
            build.AddTraitModifer(amultetDex);
            //double IT! (second won't work)
            build.AddTraitModifer(amultetDex);

            var amultetBrawl = new TraitModifier("amulet Brawl", new List<string> { Build.Abilities.Brawl }, DurationType.Scene, conditions, 2, TraitModifier.BonusTypeKind.TraitModLimited, 5);
            build.AddTraitModifer(amultetBrawl);

            var amultetAlertness = new TraitModifier("amulet Security", new List<string> { Build.Abilities.Alertness }, DurationType.Scene, conditions, 2, TraitModifier.BonusTypeKind.TraitModLimited, 5);
            //won't apply
            build.AddTraitModifer(amultetAlertness);

            info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);
            
            Assert.AreEqual(dex + brawl + 1 + 3, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1 +1, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl +2, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");

            //add bonus dices to dex
            var dexJedi = new TraitModifier("Jedi have + 5 to dex rolls", new List<string> { Build.Atributes.Dexterity }, DurationType.Scene, conditions, 5, TraitModifier.BonusTypeKind.AdditionalDice);
            build.AddTraitModifer(dexJedi);

            //all rolls in umbra have +1
            var inUmbra = new BonusModifier("in umbra", DurationType.Scene, conditions, 3);
            build.AddDicePoolBonusModifer(inUmbra);

            info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            Assert.AreEqual(dex + 1 + 1 + 5 + brawl + 2  +3, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1 + 1 + 5, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl + 2, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");
            
        }

        [Test(Description = "Test conditions")]
        public void RollTestConditionsMods()
        {
            var roller = new OfflineDiceRoller(new StringBufferLogger());
            IRollLogger logger = new StringBufferLogger();

            Build build = new Build("Artze");
            int dex = 3;
            int brawl = 3;

            build.Traits[Build.Atributes.Dexterity] = dex;
            build.Traits[Build.Abilities.Brawl] = brawl;

            string name = "Attack!";
            var targets = new List<Build> { build };
            var dicepool = new List<string> { Build.Atributes.Dexterity, Build.Abilities.Brawl };
            var conditions = new List<string>();
            bool hasSpec = true;
            bool hasWill = true;
            bool remove1 = true;

            var crinos = new TraitModifier("crinos", new List<string> { Build.Atributes.Dexterity }, DurationType.Scene, conditions, 1, TraitModifier.BonusTypeKind.TraitMod);
            build.AddTraitModifer(crinos);


            string conSunny = "Sunny";
            var sunny = new TraitModifier("Weather is sunny", new List<string> { Build.Atributes.Dexterity }, DurationType.Scene, new List<string> { conSunny }, 2, TraitModifier.BonusTypeKind.TraitMod);
            build.AddTraitModifer(sunny);


            BasicRoll roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>());
            var info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            //nothing  changed, Sunny don't apply
            Assert.AreEqual(dex + brawl + 1, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");


            roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>() { conSunny });
            info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            //nothing  changed, Sunny don't apply
            Assert.AreEqual(dex + brawl + 1 +2, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1 +2, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");

            //all rolls in umbra have +1
            string inUmbraName = "umbra";
            var inUmbra = new BonusModifier("in umbra", DurationType.Scene, new List<string>() { inUmbraName } , 3);
            build.AddDicePoolBonusModifer(inUmbra);

            roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>() { conSunny });
            info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            //nothing  changed, Umbra don't apply
            Assert.AreEqual(dex + brawl + 1 + 2, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1 + 2, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");

            //apply umbra
            roll = new BasicRoll(name, logger, roller, dicepool, remove1, true, new List<string>() { conSunny, inUmbraName });
            info = roll.GetRollInfo(build, new List<Build> { build });
            roll.Roll(build, targets, hasSpec, hasWill);

            //Umbra applied
            Assert.AreEqual(dex + brawl + 1 + 2 +3, info.DicePoolInfo.Dices, "dice pool");
            Assert.AreEqual(dex + 1 + 2 + 3, info.DicePoolInfo.Traits[Build.Atributes.Dexterity].ModifiedValue, "dex value");
            Assert.AreEqual(brawl, info.DicePoolInfo.Traits[Build.Abilities.Brawl].ModifiedValue, "brawl");



        }
    }
}


