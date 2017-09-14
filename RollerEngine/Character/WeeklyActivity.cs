using RollerEngine.Character.Party;

namespace RollerEngine.Character
{
    public class WeeklyActivity
    {
        public HatysPartyMember Teacher { get; private set; }
        public Common.Character Student { get; private set; }
        public string Trait { get; private set; }
        public string RiteName { get; private set; }
        public int RiteLevel { get; private set; }

        public WeeklyActivity(HatysPartyMember teacher, Common.Character student, string trait)
        {
            Teacher = teacher;
            Student = student;
            Trait = trait;
        }

        public WeeklyActivity(HatysPartyMember teacher, string riteName, int riteLevel)
        {
            Teacher = teacher;
            RiteName = riteName;
            RiteLevel = riteLevel;
        }
    }
}