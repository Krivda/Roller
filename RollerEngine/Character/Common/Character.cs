using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Common
{
    public class Character : IStudent
    {
        public string CharacterName { get; protected set; }
        public Build Self { get; private set; }
        public IRollLogger Log { get; set; }
        public IRoller Roller { get; set; }
        public int LearnSessions { get; set; }

        public Character(string name, Build build, IRollLogger log, IRoller roller)
        {
            Self = build;
            Self.Name = name;
            CharacterName = name;
            Log = log;
            Roller = roller;
        }

        public virtual void Learn(string ability, bool withWill)
        {
            //consume Xp from pool
            var instruct = new InstructionLearn(Log, Roller, ability);
            instruct.Roll(Self, ability, false, withWill);
        }

        public virtual void ShiftToCrinos()
        {
            CommonBuffs.ShiftToCrinos(Self, Log);
        }
    }
}