using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;

namespace RollerEngine.Character
{
    public class WeeklyActivity
    {
        public RiteInfo RiteInfo { get; protected set; }
        public bool HasSpec { get; private set; }
        public bool HasWill { get; private set; }
        public string Activity { get; protected set; }
        public HatysPartyMember Actor { get; private set; }
        public int LearnSessions { get; private set; }
        public Common.Character Student { get; private set; }
        public string Trait { get; private set; }

        public enum ActivityType
        {
            Shared,
            Exclusive
        }

        public class ActivityKind
        {
            public const string Teaching = "Teaching";          //due to cacao talen 1 month => 2 weeks
                                                                //actually this roll was decided 2 weeks ago
                                                                //this activity can be done once per 2 weeks
                                                                //shared activity

            public const string LearnTrait = "Learn Ability";
            public const string LearnRites = "Learn Rite";      //due to cacao talen you can make up to 2 learning rolls per week (3,5 days EXCLUSIVE per roll)
                                                                //due to Eidetic memory Yoki can make up to 4 learning rolls per week (1,75 days EXCLUSIVE per roll)
                                                                //due to Mind Partition Spiridon can make learning rolls as background activity
                                                                //this is EXCLUSIVE activity by default (you cannot do anything during learning)

            public const string CreateTalens = "Create Talens"; //mini umbral storm prevents fast creation; shared activity; components from modules
            public const string CreateFetish = "Create Fetish"; //mini umbral storm prevents fast creation; shared activity; components from modules
            public const string CreateDevice = "Create Device"; //shared activity; components from modules

            public const string QueueNewRite = "Queue new rite"; //add rite to learning queue

        }

        public WeeklyActivity(string activity, HatysPartyMember actor, Common.Character student, string trait)
        {
            //Teaching
            Activity = activity;
            Actor = actor;
            Student = student;
            Trait = trait;
            //TODO check once per 2 weeks; count cacao usage
        }

        public WeeklyActivity(string activity, HatysPartyMember actor, RiteInfo riteRiteInfo)
        {
            //queue rite
            Activity = activity;
            Actor = actor;

            RiteInfo = riteRiteInfo;
        }

        public WeeklyActivity(string activity, HatysPartyMember actor, int learnSessions)
        {
            //Learn rite or trait
            Activity = activity;
            Actor = actor;

            LearnSessions = learnSessions;
        }
    }
}