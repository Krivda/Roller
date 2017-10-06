using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

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

        public override void Execute()
        {
            var riteInfo = Rite.Info();
            var keyRitePool = Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, riteInfo.Name);
            var keyRiteLearned = Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned, riteInfo.Name);

            //create dynamic trait if it was absent
            if (!Actor.Self.Traits.ContainsKey(keyRitePool))
            {
                Actor.Self.Traits.Add(keyRitePool, riteInfo.SuccessesRequiredToLearn());
            }

            //create dynamic trait if it was absent
            if (!Actor.Self.Traits.ContainsKey(keyRiteLearned))
            {
                Actor.Self.Traits.Add(keyRiteLearned, 0);
            }
        }
    }
}