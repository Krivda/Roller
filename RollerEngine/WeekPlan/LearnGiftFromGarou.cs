using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class LearnGiftFromGarou : LearningActivity
    {
        public string GiftName { get; private set; }

        public LearnGiftFromGarou(HatysPartyMember actor, int maxLearnAttempts, string giftName) : base(actor, Activity.LearnGiftFromGarou, maxLearnAttempts)
        {
            GiftName = giftName;
        }
    }
}