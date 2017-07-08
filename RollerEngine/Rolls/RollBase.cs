using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RollerEngine.Character;

namespace RollerEngine.Rolls
{
    class RollBase
    {
        private readonly ILogger _log;
        private readonly Roller _roller;
        public List<Build> Targets { get; }
        public Build Actor { get; }


        public RollBase(ILogger log, Roller roller, Build actor, List<Build> targets)
        {
            _log = log;
            _roller = roller;
            Actor = actor;
            Targets = targets;
        }
    }
}
