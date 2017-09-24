using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.WeekPlan
{
    public class QueueRiteLearning : MarkerActivity
    {
        public Rite Rite { get; private set; }

        public QueueRiteLearning(HatysPartyMember actor, Rite rite) : base(actor, Activity.QueueRiteLearning)
        {
            Rite = rite;
        }
    }
}