using System;
using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;
using RollerEngine.WodSystem;
using RollerEngine.WodSystem.WTA;

namespace RollerEngine.WeekPlan
{
    public abstract class TeachingActivity : WeeklyActivity
    {
        public IStudent Student { get; private set; }

        protected TeachingActivity(HatysPartyMember actor, IStudent student, Activity activity, int delay) : base(actor, activity, ActivityType.Single, ActivityKind.Teaching, delay)
        {
            Student = student;
        }
    }

    public class TeachAbility : TeachingActivity
    {
        public string Ability { get; private set; }

        public TeachAbility(HatysPartyMember actor, IStudent student, string ability)
            : base(actor, student, Activity.TeachAbility, 0 /*-4 or -2*/)
        {
            Ability = ability;
        }

        public override void Execute()
        {
            Actor.Instruct(Student.Self, Ability, false);
        }
    }

    public class TeachGiftToGarou : TeachingActivity
    {
        public string Gift { get; private set; }
        public int Level { get; private set; }

        public TeachGiftToGarou(HatysPartyMember actor, IStudent student, string gift, int level)
            : base(actor, student, Activity.TeachGiftToGarou, 0 /*-4 or -2*/)
        {
            //TODO need GIFT Dictionary same as for Rites
            //TODO need FETISH Dictionary same as for Rites
            Gift = gift;
            Level = Level;
        }

        public override void Execute()
        {
            throw new NotImplementedException("TeachGiftToGarou.Execute not implemented");
            //Actor.Instruct(Student.Self, Gift, Level, false);
        }
    }

    public class TeachRiteToGarou : TeachingActivity
    {
        public Rite Rite { get; private set; }
        public int MaxLearnAttempts { get; protected set; }

        public TeachRiteToGarou(HatysPartyMember actor, IStudent student, Rite rite, int maxLearnAttempts)
            : base(actor, student, Activity.TeachRiteToGarou, 0)
        {
            Rite = rite;
            MaxLearnAttempts = maxLearnAttempts; //actor and student attempts should be synced
        }
    }

}