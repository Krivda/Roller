using RollerEngine.Character.Common;
using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class TeachAbility : TeachingActivity
    {
        public string Ability { get; private set; }

        public TeachAbility(HatysPartyMember actor, IStudent student, string ability) : base(actor, student, Activity.TeachAbility, ActivityType.Single, 0 /*-2*/)
        {
            Ability = ability;
        }
    }
}