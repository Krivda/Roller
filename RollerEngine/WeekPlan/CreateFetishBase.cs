using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateFetishBase : CreationActivity
    {
        public string FetishName { get; private set; }
        public int Level { get; private set; }

        public CreateFetishBase(HatysPartyMember actor, int fetishLevel, string fetishName) : base(actor, Activity.CreateFetishBase, ActivityType.Single, 0)
        {
            FetishName = fetishName;
            Level = fetishLevel;
        }
    }
}