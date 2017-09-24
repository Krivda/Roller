using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public class CreateDevice : CreationActivity
    {
        public string DeviceName { get; private set; }

        public CreateDevice(HatysPartyMember actor, string deviceName) : base(actor, Activity.CreateDevice, ActivityType.Extended, 0)
        {
            DeviceName = deviceName;
        }
    }
}