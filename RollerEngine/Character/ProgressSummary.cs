using System;
using System.Collections.Generic;

namespace RollerEngine.Character
{
    internal class ProgressSummary
    {
        //               ability             key(dyn)      was now
        public Dictionary<string, Dictionary<string, Tuple<int, int>>> AbilityProgress { get; private set; }
        //               riteName       was  now  target 
        public Dictionary<string, Tuple<int, int, int>> RiteProgress { get; private set; }

        public Dictionary<string, Tuple<int, int>> ItemsProgress { get; private set; }

        public ProgressSummary()
        {
            AbilityProgress = new Dictionary<string, Dictionary<string, Tuple<int, int>>>();
            RiteProgress = new Dictionary<string, Tuple<int, int, int>>();
            ItemsProgress = new Dictionary<string, Tuple<int, int>>();
        }
    }
}