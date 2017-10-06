using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public abstract class CreationActivity : WeeklyActivity
    {
        //TODO: count components from modules
        protected CreationActivity(HatysPartyMember actor, Activity activity, ActivityType type, int delay) : base(actor, activity, type, ActivityKind.Creation, delay)
        {
        }
    }

    public class CreateDeviceActivity : CreationActivity
    {
        public string DeviceName { get; private set; }

        public CreateDeviceActivity(HatysPartyMember actor, string deviceName)
            : base(actor, Activity.CreateDevice, ActivityType.Extended, 0)
        {
            DeviceName = deviceName;
        }
    }

    public class CreateItemActivity : CreationActivity
    {
        public string FetishName { get; private set; }
        public int Level { get; private set; }

        public CreateItemActivity(HatysPartyMember actor, int fetishLevel, string fetishName)
            : base(actor, Activity.CreateItem, ActivityType.Single, 0)
        {
            FetishName = fetishName;
            Level = fetishLevel;
        }
    }

    public class CreateTalensActivity : CreationActivity
    {
        public string TalenName { get; private set; }
        public string SpiritType { get; private set; }

        public CreateTalensActivity(HatysPartyMember actor, string talenName, string spiritType) :
            base(actor, Activity.CreateTalens, ActivityType.Single, 0 /*0.5*/)
        {
            TalenName = talenName;
            SpiritType = spiritType;
        }
    }

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