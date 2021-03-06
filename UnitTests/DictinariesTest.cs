﻿using System;
using System.Linq;
using NLog;
using NUnit.Framework;
using RollerEngine.Logger;
using RollerEngine.WodSystem.WTA;

namespace UnitTests
{
    internal class DictinariesTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestRites()
        {
            IRollLogger logger = CompositeLogger.InitLogging(Verbosity.Debug, null, null, null);

            foreach (var rite in Enum.GetValues(typeof(Rite)).Cast<Rite>())
            {
                var riteInfo = rite.Info();

                var conditions = riteInfo.Conditions.Aggregate("", (current, condition) =>
                    string.Format("{0}{1}{2}", current, string.IsNullOrEmpty(current) ? "" : " ,", condition));

                logger.Log(Verbosity.Critical,  string.Format(
                    "{0}: Rite of {1}, level={2}, group={3} conditions={4}", riteInfo.Rite, riteInfo.Name, riteInfo.Level, riteInfo.Group, conditions));

            }

        }
    }
}
