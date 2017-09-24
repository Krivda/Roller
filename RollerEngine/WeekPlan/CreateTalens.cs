using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateTalens : CreationActivity
    {
        public string TalenName { get; private set; }
        public int MaxCreationAttempts { get; protected set; }

        public CreateTalens(HatysPartyMember actor, string talenName, int maxCreationAttempts) : base(actor, Activity.CreateTalens, ActivityType.Single, 0 /*0.5*/)
        {
            TalenName = talenName;
            if (maxCreationAttempts > 2)
            {
                maxCreationAttempts = 2;
            }
            MaxCreationAttempts = maxCreationAttempts;
        }
    }
}