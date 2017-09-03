using RollerEngine.Character.Party;

namespace RollerEngine.Character
{
    public class TeachPlan
    {
        public HatysPartyMember Teacher { get; private set; }
        public Common.Character Student { get; private set; }
        public string Trait { get; private set; }

        public TeachPlan(HatysPartyMember teacher, Common.Character student, string trait)
        {
            Teacher = teacher;
            Student = student;
            Trait = trait;
        }
    }
}