using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

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

    public class LearnGiftFromGarou : LearningActivity
    {
        public string GiftName { get; private set; }

        public LearnGiftFromGarou(HatysPartyMember actor, string giftName, int maxLearnAttempts)
            : base(actor, Activity.LearnGiftFromGarou, maxLearnAttempts)
        {
            GiftName = giftName;
        }
    }

    public class LearnRiteFromGarou : LearningActivity
    {
        public Rite Rite { get; private set; }
        public ITeacher Teacher { get; private set; }

        public LearnRiteFromGarou(HatysPartyMember actor, ITeacher teacher, Rite rite, int maxLearnAttempts)
            : base(actor, Activity.LearnRiteFromGarou, maxLearnAttempts)
        {
            Rite = rite;
            Teacher = teacher;
        }
    }
}