using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.WeekPlan
{
    public class LearnRiteFromGarou : LearningActivity
    {
        public Rite Rite { get; private set; }

        public LearnRiteFromGarou(HatysPartyMember actor, int maxLearnAttempts, Rite rite) : base(actor, Activity.LearnRiteFromGarou, maxLearnAttempts)
        {
            Rite = rite;
        }
    }
}