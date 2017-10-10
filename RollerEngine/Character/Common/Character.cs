using System;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Common
{
    public class Character : IStudent, ITeacher
    {
        public string CharacterName { get; protected set; }
        public Build Self { get; private set; }
        public IRollLogger Log { get; set; }
        public IRollAnalyzer Roller { get; set; }
        public int WeeklyPartialActions { get; set; }
        public int WeeklyActions { get; set; }

        public Character(string name, Build build, IRollLogger log, IRollAnalyzer roller)
        {
            Self = build;
            Self.Name = name;
            CharacterName = name;
            Log = log;
            Roller = roller;
            WeeklyPartialActions = 1;
        }

        public virtual void LearnAbility(string ability, bool withWill)
        {
            //consume Xp from pool
            var instruct = new InstructionLearn(Log, Roller, ability);
            instruct.Roll(Self, ability, false, withWill);
        }

        public virtual void Instruct(Build target, string ability, bool withWill)
        {
            throw new Exception("abstract ");
        }

        public void Teach(IStudent student, string ability, bool hasWill)
        {
            Instruct(student.Self, ability, hasWill);
        }

        public virtual void ShiftToCrinos()
        {
            CommonBuffs.ShiftToCrinos(Self, Log);
        }
    }
}