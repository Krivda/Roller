using System.Collections.Generic;

namespace RollerEngine.Extensions
{
    public static class ListExtension
    {
        public static void AddNewElements<T>(List<T> fromList, List<T> whatList)
        {
            foreach (var element in whatList)
            {
                if (!fromList.Contains(element)) fromList.Add(element);
            }
        }

        public static void RemoveElements<T>(List<T> fromList, List<T> whatList)
        {
            foreach (var element in whatList)
            {
                if (fromList.Contains(element)) fromList.Remove(element);
            }
        }

    }
}