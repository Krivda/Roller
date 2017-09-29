using System.Collections.Generic;
using System.Linq;

namespace RollerEngine.WeekPlan
{
    public static class WeeklyFilter
    {
        private static List<T> ByActivity<T>(List<WeeklyActivity> plan, Activity activity)
        {
            var filtered = plan.Where(item => item.Activity == activity).Cast<T>().ToList();
            return filtered;
        }

        public static List<QueueRiteLearning> ByQueueRiteLearning(List<WeeklyActivity> plan)
        {
            return ByActivity<QueueRiteLearning>(plan, Activity.QueueRiteLearning);
        }

        public static List<CreateTalens> ByCreateTalens(List<WeeklyActivity> plan)
        {
            return ByActivity<CreateTalens>(plan, Activity.CreateTalens);
        }

        public static List<CreateFetishActivity> ByCreateFetish(List<WeeklyActivity> plan)
        {
            return ByActivity<CreateFetishActivity>(plan, Activity.CreateFetish);
        }

        public static List<CreateFetishBase> ByCreateFetishBase(List<WeeklyActivity> plan)
        {
            return ByActivity<CreateFetishBase>(plan, Activity.CreateFetishBase);
        }

        public static List<CreateDevice> ByCreateDevice(List<WeeklyActivity> plan)
        {
            return ByActivity<CreateDevice>(plan, Activity.CreateDevice);
        }

        public static List<TeachAbility> ByTeachAbility(List<WeeklyActivity> plan)
        {
            return ByActivity<TeachAbility>(plan, Activity.TeachAbility);
        }

        public static List<TeachGiftToGarou> ByTeachGiftToGarou(List<WeeklyActivity> plan)
        {
            return ByActivity<TeachGiftToGarou>(plan, Activity.TeachGiftToGarou);
        }

        public static List<TeachRiteToGarou> ByTeachRiteToGarou(List<WeeklyActivity> plan)
        {
            return ByActivity<TeachRiteToGarou>(plan, Activity.TeachRiteToGarou);
        }

        public static List<LearnAbility> ByLearnAbility(List<WeeklyActivity> plan)
        {
            return ByActivity<LearnAbility>(plan, Activity.LearnAbility);
        }

        public static List<LearnRite> ByLearnRite(List<WeeklyActivity> plan)
        {
            return ByActivity<LearnRite>(plan, Activity.LearnRite);
        }

        public static List<LearnGiftFromGarou> ByLearnGiftFromGarou(List<WeeklyActivity> plan)
        {
            return ByActivity<LearnGiftFromGarou>(plan, Activity.LearnGiftFromGarou);
        }

        public static List<LearnRiteFromGarou> ByLearnRiteFromGarou(List<WeeklyActivity> plan)
        {
            return ByActivity<LearnRiteFromGarou>(plan, Activity.LearnRiteFromGarou);
        }

        private static List<T> ByKind<T>(List<WeeklyActivity> plan, ActivityKind type)
        {
            var filtered = plan.Where(item => item.Kind == type).Cast<T>().ToList();
            return filtered;
        }

        public static List<MarkerActivity> ByMarker(List<WeeklyActivity> plan)
        {
            return ByKind<MarkerActivity>(plan, ActivityKind.None);
        }

        public static List<CreationActivity> ByCreation(List<WeeklyActivity> plan)
        {
            return ByKind<CreationActivity>(plan, ActivityKind.Creation);
        }

        public static List<TeachingActivity> ByTeaching(List<WeeklyActivity> plan)
        {
            return ByKind<TeachingActivity>(plan, ActivityKind.Teaching);
        }

        public static List<LearningActivity> ByLearning(List<WeeklyActivity> plan)
        {
            return ByKind<LearningActivity>(plan, ActivityKind.Learning);
        }

        public static List<T> ByType<T>(List<T> plan, ActivityType type)
            where T : WeeklyActivity
        {
            var filtered = plan.Where(item => item.Type == type).ToList();
            return filtered;
        }

        public static List<T> ByActor<T>(List<T> plan, string actorName)
            where T : WeeklyActivity
        {
            var filtered = plan.FindAll(item => item.Actor.CharacterName.Equals(actorName));
            return filtered;
        }
    }
}