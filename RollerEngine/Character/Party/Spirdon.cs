using RollerEngine.Logger;
using RollerEngine.Roller;

namespace RollerEngine.Character.Party
{
    public class Spirdon
    {
        private readonly HatysParty _party;
        public Build Build { get; private set; }
        public IRollLogger Log { get; private set; }
        public IRoller Roller { get; private set; }
        public const string CharacterName = "Спиридон";

        public Spirdon(Build build, IRollLogger log, IRoller roller, HatysParty party)
        {
            _party = party;
            Build = build;
            Build.Name = CharacterName;
            Log = log;
            Roller = roller;
        }
    }
}