using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateFetishBase : CreationActivity
    {
        public string FetishName { get; private set; }

        public CreateFetishBase(HatysPartyMember actor, string fetishName) : base(actor, Activity.CreateFetishBase, ActivityType.Extended, 0)
        {
            FetishName = fetishName;
        }
    }
}