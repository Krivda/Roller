using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RollerEngine.Roller;

namespace UnitTests
{
    public class RollerTest
    {
        private MethodInfo GetMethod(Type clazz, string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                Assert.Fail("methodName cannot be null or whitespace");

            var method = clazz.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
                Assert.Fail("{0} method not found", methodName);

            return method;
        }


        [Test]
        public void RollerTestSimple()
        {

            var meth = GetMethod(typeof(RollAnalyzer), "GetRollStats");
            bool hasSpec = false;
            bool hasWill = false;
            bool remove1 = false;
            int DC = 6;

            var roll = new List<int>();

            roll.Add(0);
            roll.Add(1);
            roll.Add(0);
            roll.Add(2);
            roll.Add(4);
            roll.Add(0);
            roll.Add(0);
            roll.Add(0);
            roll.Add(0);
            roll.Add(0);

            //private static int GetRollSuccesses(List<int> rawResult, int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
            int res = ((RollData)meth.Invoke(null, new Object[]{ roll, roll.Sum(), DC, remove1, hasSpec, hasWill } )).Successes;

            Assert.AreEqual(0, res, "бросок без успехов");

            roll[5] = 1; //6
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(1, res, "бросок 1 успех");


            roll[6] = 1; //7
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(2, res, "2 succ");

            DC = 7;
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(1, res, "1 succ vs 7");

            roll[0] = 1; //1
            DC = 7;
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(1, res, "1 succ vs 7 w/o substract 1");

            remove1 = true;
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(0, res, "0 succ vs 7 with substract 1");

            roll[9] = 1; //10
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(1, res, "1 succ vs 7 wo spec wuth sub");

            hasSpec = true;
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(2, res, "2 succ vs 7 with spec");

            roll[0] = 2; //10
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(1, res, "1 succ vs 7 with spec");

            roll[9] = 2; //10
            res = ((RollData)meth.Invoke(null, new Object[] { roll, roll.Sum(), DC, remove1, hasSpec, hasWill })).Successes;
            Assert.AreEqual(3, res, "3 succ vs 7 with spec");
        }
    }
}