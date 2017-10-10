using System;
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

            var meth = GetMethod(typeof(RollAnalyzer), "GetRollSuccesses");
            bool hasSpec = false;
            bool hasWill = false;
            bool remove1 = false;
            int DC = 6;
            

            int[] roll = new int[10];

            roll[0] = 0;
            roll[1] = 1;
            roll[2] = 0;
            roll[3] = 2;
            roll[4] = 4;
            roll[5] = 0;
            roll[6] = 0;
            roll[7] = 0;
            roll[8] = 0;
            roll[9] = 0;

            //private static int GetRollSuccesses(int[] roll, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill)
            int res = (int)meth.Invoke(null, new Object[]{ roll, DC, remove1, hasSpec, hasWill } );

            Assert.AreEqual(0, res, "бросок без успехов");

            roll[5] = 1; //6
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(1, res, "бросок 1 успех");


            roll[6] = 1; //7
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(2, res, "2 succ");

            DC = 7;
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(1, res, "1 succ vs 7");

            roll[0] = 1; //1
            DC = 7;
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(1, res, "1 succ vs 7 w/o substract 1");

            remove1 = true;
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(0, res, "0 succ vs 7 with substract 1");

            roll[9] = 1; //10
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(1, res, "1 succ vs 7 wo spec wuth sub");

            hasSpec = true;
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(2, res, "2 succ vs 7 with spec");

            roll[0] = 2; //10
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(1, res, "1 succ vs 7 with spec");

            roll[9] = 2; //10
            res = (int)meth.Invoke(null, new Object[] { roll, DC, remove1, hasSpec, hasWill });
            Assert.AreEqual(3, res, "3 succ vs 7 with spec");
        }
    }
}