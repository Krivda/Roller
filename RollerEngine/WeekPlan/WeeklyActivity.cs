using System;
using System.Collections.Generic;
using RollerEngine.Character.Party;

namespace RollerEngine.WeekPlan
{
    public enum ActivityType
    {
        None,
        Single,
        Extended
    }

    public enum ActivityKind
    {
        None,
        Creation,
        Teaching,
        Learning
    }

    public enum Activity
    {
        //due to Mind Partition you can make extended activities in background

        //none activities
        QueueRiteLearning, //None; add rite to learning queue (with priority)

        //creation activities:
        CreateTalens,     //Single; Delay; item is required (mini umbral storm prevents talen creation more one per week)
        CreateFetish,     //Single; Delay; item is required (mini umbral storm prevents creation for 1/2/3 weeks (1-2/3-4/5-6 fetish level);
        CreateItem,       //Extended; components from modules
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
        public Activity Activity { get; private set; }
        public ActivityType Type { get; private set; }
        public ActivityKind Kind { get; private set; }
        public int Delay { get; private set; }             //block for several weeks this activity

        protected static Dictionary<Activity, int> Delays = new Dictionary<Activity, int>();

        protected WeeklyActivity(HatysPartyMember actor, Activity activity, ActivityType type, ActivityKind kind, int delay)
        {
            Actor = actor;
            Activity = activity;
            Type = type;
            Kind = kind;
            Delay = delay;
        }

        public virtual void Execute()
        {
            throw new NotImplementedException("WeeklyActivity.Execute should be overriden");
        }
    }
}