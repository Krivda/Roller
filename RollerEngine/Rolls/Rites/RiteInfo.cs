using System.Collections.Generic;

namespace RollerEngine.Rolls.Rites
{
    public class RiteInfo
    {
        public string Name { get; private set; }
        public double Level { get; private set; }
        public string Group { get; private set; }
        public List<string> Conditions { get; private set; }

        public RiteInfo(string riteName, double riteLevel, string riteGroup, List<string> conditions)
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

            someRite = new RiteInfo("of Opened Caern", 1, "Caern", new List<string>());
            Rites.Add(someRite.Name, someRite);

            someRite = new RiteInfo("of Talisman Adaptation", 3, "Mystic", new List<string>());
            Rites.Add(someRite.Name, someRite);

            someRite = new RiteInfo("of Ancestors Veneration", 0.5, "Minor", new List<string>());
            Rites.Add(someRite.Name, someRite);
        }
    }
}
