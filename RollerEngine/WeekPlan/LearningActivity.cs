using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public abstract class LearningActivity : WeeklyActivity
    {
        public int MaxLearnAttempts { get; protected set; }

        protected LearningActivity(HatysPartyMember actor, Activity activity, int maxLearnAttempts) : base(actor, activity, ActivityType.Extended, ActivityKind.Learning, 0)
        {
            MaxLearnAttempts = maxLearnAttempts;
        }
    }

    public class LearnRite : LearningActivity
    {
        public LearnRite(HatysPartyMember actor, int maxLearnAttempts) : base(actor, Activity.LearnRite, maxLearnAttempts)
        {
        }
    }

    public class LearnAbility : LearningActivity
    {
        public LearnAbility(HatysPartyMember actor, int maxLearnAttempts) : base(actor, Activity.LearnAbility, maxLearnAttempts)
        {
        }
    }
}