using System.Collections.Generic;

namespace RollerEngine.Rolls.Rites
{
    public class RiteInfo
    {
        public string Name { get; private set; }
        public int Level { get; private set; }
        public string Group { get; private set; }
        public List<string> Conditions { get; private set; }

        public RiteInfo(string riteName, int riteLevel, string riteGroup, List<string> conditions)
        {
            Name = riteName;
            Level = riteLevel;
            Group = riteGroup;
            Conditions = conditions;
        }
    }


    public class RitesDictionary
    {
        public static Dictionary<string, RiteInfo> Rites { get; private set; }

        static RitesDictionary()
        {
            Rites = new Dictionary<string, RiteInfo>();

            var someRite = new RiteInfo("Some rite 1", 5, "Mystic", new List<string>());
            Rites.Add(someRite.Name, someRite);
            someRite = new RiteInfo("Some rite 2", 4, "Mystic", new List<string>());
            Rites.Add(someRite.Name, someRite);
            someRite = new RiteInfo("Rite of Something", 5, "Mystic", new List<string>());
            Rites.Add(someRite.Name, someRite);
        }
    }
}
