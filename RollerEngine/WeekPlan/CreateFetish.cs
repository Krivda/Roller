using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateFetishActivity : CreationActivity
    {
        public string FetishName { get; private set; }
        public string SpiritType { get; private set; }
        public int Level { get; private set; }

        public CreateFetishActivity(HatysPartyMember actor, int level, string fetishName, string spiritType) : 
            base(actor, Activity.CreateFetish, ActivityType.Single, 0 /*level/2*/)
        {
            //TODO: assert level 1-6
            FetishName = fetishName;
            SpiritType = spiritType;
            Level = level;
        }
    }
}