using RollerEngine.Character.Common;

namespace UnitTests
{
    using NLog;
    using NUnit.Framework;
    using RollerEngine.Logger;
    using RollerEngine.Roller;
    using RollerEngine.Rolls.Skills;

    class InstructionTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestInstructionTeach()
        {
            var rollLogger = new NLogLogger(Logger);
            var roller = new MockFixedRoller(new NLogLogger(Logger));

            Build teacher = new Build("Teacher");
            teacher.Traits[Build.Abilities.Instruction] = 3;
            teacher.Traits[Build.Abilities.Occult] = 4;

            Build student = new Build("Student");
            student.Traits[Build.Abilities.Occult] = 0;

            string xpPoolOccult =
                Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpiriencePool, Build.Abilities.Occult);
            string xpConsumedOcuult =
                Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned, Build.Abilities.Occult);

            var rollTeach = new InstructionTeach(rollLogger, roller);
            roller.Successes = 5;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(5, student.Traits[xpPoolOccult], "Student should get 5 XP on occult");

            roller.Successes = 4;
            var rollLearn = new InstructionLearn(rollLogger, roller, Build.Abilities.Occult);

            //student should learn 1 occult and have 2 more XP
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            //got 5 learned 4 up to the point (1 occult + 1 xp pool, 1 exp learned)
            Assert.AreEqual(1, student.Traits[Build.Abilities.Occult], "Student should have learned 1 Occult");
            Assert.AreEqual(1, student.Traits[xpPoolOccult], "Student should have 2 more XP on  to spend on Occult");
            Assert.AreEqual(1, student.Traits[xpConsumedOcuult],
                "Student should have 1 XP learned towards a point in Occult.");

            //learn 1 more XP: it's not enought to learn a point
            roller.Successes = 1;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            //got 5 learned 5 up to the point (1 occult + 1 xp pool, 2 exp learned)
            Assert.AreEqual(1, student.Traits[Build.Abilities.Occult], "Student should have learned 1 Occult");
            Assert.AreEqual(0, student.Traits[xpPoolOccult], "Student should have 0 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");


            //have no pool to futher consume
            roller.Successes = 5;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(1, student.Traits[Build.Abilities.Occult], "Student should have learned 1 Occult");
            Assert.AreEqual(0, student.Traits[xpPoolOccult], "Student should have 0 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");

            //learn 1 more occult
            // teach 2 more xp
            roller.Successes = 2;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(1, student.Traits[Build.Abilities.Occult], "Student should have learned 1 Occult");
            Assert.AreEqual(2, student.Traits[xpPoolOccult], "Student should have 2 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");

            //learn 3 (2 is effective, 1 exp is lost)
            // + 1 occult, 0 in pool, 0 learned
            roller.Successes = 3;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(0, student.Traits[xpPoolOccult], "Student should have 0 more XP on  to spend on Occult");
            Assert.AreEqual(0, student.Traits[xpConsumedOcuult],
                "Student should have 0 XP learned towards a point in Occult.");

            // teach 2 more xp
            roller.Successes = 2;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(2, student.Traits[xpPoolOccult], "Student should have 2 more XP on  to spend on Occult");
            Assert.AreEqual(0, student.Traits[xpConsumedOcuult],
                "Student should have 0 XP learned towards a point in Occult.");


            //learn 3 (2 is effective, 1 exp is lost)
            //1 occult, 0 in pool, 2 learned
            roller.Successes = 3;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(0, student.Traits[xpPoolOccult], "Student should have 0 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");

            // teach 2 more xp 
            roller.Successes = 2;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(2, student.Traits[xpPoolOccult], "Student should have 2 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");

            //teach 3 more successes. only 2 should be effective (limited by teachers Instruction)
            roller.Successes = 3;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(4, student.Traits[xpPoolOccult], "Student should have 4 more XP on  to spend on Occult");
            Assert.AreEqual(2, student.Traits[xpConsumedOcuult],
                "Student should have 2 XP learned towards a point in Occult.");

            // learn to consume 3 XP from pool 
            roller.Successes = 3;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(1, student.Traits[xpPoolOccult], "Student should have 1 more XP on  to spend on Occult");
            Assert.AreEqual(5, student.Traits[xpConsumedOcuult],
                "Student should have 5 XP learned towards a point in Occult.");

            //raising oinstruction to 4
            teacher.Traits[Build.Abilities.Instruction] = 4;

            //max pool
            roller.Successes = 30;
            rollTeach.Roll(teacher, student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(2, student.Traits[Build.Abilities.Occult], "Student should have learned 2 Occult");
            Assert.AreEqual(9, student.Traits[xpPoolOccult], "Student should have 9 more XP on  to spend on Occult");
            Assert.AreEqual(5, student.Traits[xpConsumedOcuult],
                "Student should have 5 XP learned towards a point in Occult.");


            // max learn (2->4 in occult)
            roller.Successes = 10;
            rollLearn.Roll(student, Build.Abilities.Occult, false, false);

            Assert.AreEqual(4, student.Traits[Build.Abilities.Occult], "Student should have learned 4 Occult");
            Assert.AreEqual(0, student.Traits[xpPoolOccult], "Student should have 0 more XP on  to spend on Occult");
            Assert.AreEqual(0, student.Traits[xpConsumedOcuult],
                "Student should have 0 XP learned towards a point in Occult.");
        }
    }
}