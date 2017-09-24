using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.WeekPlan
{
    public class TeachRiteToGarou : TeachingActivity
    {
        public Rite Rite { get; private set; }
        public int MaxLearnAttempts { get; protected set; }

        public TeachRiteToGarou(HatysPartyMember actor, IStudent student, Rite rite, int maxLearnAttempts) : base(actor, student, Activity.TeachRiteToGarou, ActivityType.Single, 0)
        {
            Rite = rite;
            MaxLearnAttempts = maxLearnAttempts; //actor and student attempts should be synced
        }
    }
}