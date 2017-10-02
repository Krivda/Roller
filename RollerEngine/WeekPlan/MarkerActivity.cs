using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.WeekPlan
{
    // activity to add something in plan (takes no action, i.e. schedule rite learning)
    public abstract class MarkerActivity : WeeklyActivity
    {
        protected MarkerActivity(HatysPartyMember actor, Activity activity) : base(actor, activity, ActivityType.None, ActivityKind.None, 0)
        {
        }
    }

    public class QueueRiteLearning : MarkerActivity
    {
        public Rite Rite { get; private set; }

        public QueueRiteLearning(HatysPartyMember actor, Rite rite)
            : base(actor, Activity.QueueRiteLearning)
        {
            Rite = rite;
        }
    }
}