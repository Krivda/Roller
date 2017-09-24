using RollerEngine.Character.Common;
using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public enum ActivityType
    {
        None,
        Single,     //can't make single & exteneded simultaniously
        Extended    //can't make extended & extened simultaniously
    }

    public enum ActivityKind
    {
        None,
        Creation,   //can't make creation & creation simultaniously
        Teaching,   //can't make teaching & teaching simultaniously
        Learning    //can't make learning & learning simultaniously
    }

    public enum Activity
    {
        //due to Mind Partition you can make extended activities in background

        //none activities
        //QueueAbility, //TODO: (AI) later when will be able to find tacher add ability to learning queue (with priority)
        QueueRiteLearning, //None; add rite to learning queue (with priority)

        //creation activities:
        CreateTalens,     //Single; Delay; components from modules, mini umbral storm prevents talen creation more than twice per week;
        CreateFetish,     //Single; Delay; components from modules, mini umbral storm prevents creation for 1/2/3 weeks (1-2/3-4/5-6 fetish level);
        CreateFetishBase, //Extended; components from modules
        CreateDevice,     //Extended; components from modules

        //teaching activities (halving duration due to Cacao):
        TeachAbility,     //Delay; Single - for Abilities/Gifts (once per 2 weeks)
        TeachGiftToGarou, //Delay; Single - for Abilities/Gifts (once per 2 weeks)
        TeachRiteToGarou, //Single; taken every time student takes extended learning - for Rites

        //learning activities (halving duration due to Cacao or 1/4 due to Eidetic Memory till June 2017):
        LearnAbility,      //Extended;
        LearnRite,         //Extended;
        LearnGiftFromGarou,//Extended;
        LearnRiteFromGarou //Extended;

    }

    public abstract class WeeklyActivity
    {
        public HatysPartyMember Actor { get; private set; }
        public Activity Activity { get; protected set; }
        public ActivityType Type { get; private set; }
        public ActivityKind Kind { get; private set; }
        public int Delay;                                       //block for next this activity

        protected WeeklyActivity(HatysPartyMember actor, Activity activity, ActivityType type, ActivityKind kind, int delay)
        {
            Actor = actor;
            Activity = activity;
            Type = type;
            Kind = kind;
            Delay = delay;
        }

    }

    // activity to add something in plan (takes no action, i.e. schedule rite learning)
    public abstract class MarkerActivity : WeeklyActivity
    {
        protected MarkerActivity(HatysPartyMember actor, Activity activity) : base(actor, activity, ActivityType.None, ActivityKind.None, 0)
        {
        }
    }

    public abstract class CreationActivity : WeeklyActivity
    {
        //TODO: count components from modules
        protected CreationActivity(HatysPartyMember actor, Activity activity, ActivityType type, int delay) : base(actor, activity, type, ActivityKind.Creation, delay)
        {
        }
    }

    public abstract class TeachingActivity : WeeklyActivity
    {
        public IStudent Student { get; private set; }

        protected TeachingActivity(HatysPartyMember actor, IStudent student, Activity activity, ActivityType type, int delay) : base(actor, activity, type, ActivityKind.Teaching, delay)
        {
            Student = student;
        }
    }
}