using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using NLog;
using NUnit.Framework;
using RollerEngine.Rolls.Backgrounds;

namespace UnitTests
{
/*
    class AnsestorsTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void AnscestorsTest()
        {
            var roller = new RollAnalyzer(new MockFixedRoller());
            IRollLogger logger = CompositeLogger.InitLogging(Verbosity.Debug, null, null, null);

            logger.Log(Verbosity.Debug,  "<== Ancestors Test");

            Build user = new Build("Teacher");
            user.Traits[Build.Backgrounds.Ancestors] = 3;

            user.AncestorsUsesLeft = 1;

            roller.Successes = 4;

            var roll = new Ancestors(logger, roller, Verbosity.Critical);
            roll.Roll(user, Build.Abilities.Empathy);

            Assert.AreEqual(true, user.TraitModifiers.Any(tm => tm.Traits.Contains(Build.Abilities.Empathy) && tm.Name.Equals(Build.Backgrounds.Ancestors)), "Empathy have 4 bonus after Ancestors use!");

            //simulate botch
            user.AncestorsUsesLeft = 50;

            roller.Successes = -1;
            roll = new Ancestors(logger, roller, Verbosity.Critical);
            roll.Roll(user, Build.Abilities.Occult);
            //not recevied mod
            Assert.AreEqual(false, user.TraitModifiers.Any(tm => tm.Traits.Contains(Build.Abilities.Occult) && tm.Name.Equals(Build.Backgrounds.Ancestors)), "Occult shouldn't have 4 bonus after Ancestors use!");
            Assert.AreEqual(0, user.AncestorsUsesLeft, "botch should have spoiled further attempts");

            user.AncestorsUsesLeft = 50;
            user.HasAncestorVeneration = true;
            //test veneration fixes 
            var roller2 = new MockFixedRoller(LoggerFactory.CreateNLogLogger(Logger), new List<int>(){-1, 1});
            roll = new Ancestors(logger, roller2, Verbosity.Critical);
            roll.Roll(user, Build.Abilities.Occult);
            //not recevied mod
            Assert.AreEqual(true, user.TraitModifiers.Any(tm => tm.Traits.Contains(Build.Abilities.Occult) && tm.Name.Equals(Build.Backgrounds.Ancestors)), "Occult should have a bonus after Ancestors use!");
            Assert.AreEqual(1, user.TraitModifiers.Find(tm => tm.Traits.Contains(Build.Abilities.Occult) && tm.Name.Equals(Build.Backgrounds.Ancestors)).Value, "Occult should have 1 bonus after Ancestors use!");
            Assert.AreEqual(50, user.AncestorsUsesLeft, "botch should have 50 further attempts");

            var roller3 = new MockFixedRoller(LoggerFactory.CreateNLogLogger(Logger), new List<int>() { -1, 0 });
            roll = new Ancestors(logger, roller3, Verbosity.Critical);
            roll.Roll(user, Build.Abilities.Rituals);
            //not recevied mod
            Assert.AreEqual(false, user.TraitModifiers.Any(tm => tm.Traits.Contains(Build.Abilities.Rituals) && tm.Name.Equals(Build.Backgrounds.Ancestors)), "Rituals should have a bonus after Ancestors use!");
            Assert.AreEqual(50, user.AncestorsUsesLeft, "botch should have 50 further attempts");
        }
    }
*/
}