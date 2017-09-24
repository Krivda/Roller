using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateFetish : CreationActivity
    {
        public string FetishName { get; private set; }
        public int Level { get; private set; }
        public int MaxCreationAttempts { get; protected set; }

        public CreateFetish(HatysPartyMember actor, string fetishName, int level) : base(actor, Activity.CreateDevice, ActivityType.Single, 0 /*level/2*/)
        {
            //TODO: assert level 1-6
            FetishName = fetishName;
            Level = level;
            MaxCreationAttempts = 1;
        }
    }
}