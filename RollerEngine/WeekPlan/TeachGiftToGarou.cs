using RollerEngine.Character.Common;
using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class TeachGiftToGarou : TeachingActivity
    {
        public string Gift { get; private set; }

        public TeachGiftToGarou(HatysPartyMember actor, IStudent student, string gift) : base(actor, student, Activity.TeachGiftToGarou, ActivityType.Single, 0 /*-2*/)
        {
            Gift = gift;
        }
    }
}