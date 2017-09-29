using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateTalens : CreationActivity
    {
        public string TalenName { get; private set; }
        public string SpiritType { get; private set; }

        public CreateTalens(HatysPartyMember actor, string talenName, string spiritType) : 
            base(actor, Activity.CreateTalens, ActivityType.Single, 0 /*0.5*/)
        {
            TalenName = talenName;
            SpiritType = spiritType;
        }
    }
}