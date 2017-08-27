using System.Collections.Generic;
using RollerEngine.Character.Party;
using RollerEngine.Roller;
using RollerEngine.Logger;

namespace RollerEngine.Character
{
    public class HatysParty
    {
        public Nameless Nameless { get; private set; }
        public Spirdon Spirdon { get; private set; }
        public Yoki Yoki { get; private set; }
        public Kurt Kurt { get; private set; }

        public HatysParty(Dictionary<string, Build> party, IRollLogger log, IRoller roller)
        {
            Nameless = new Nameless(party["Krivda"], log, roller, this);
            Spirdon = new Spirdon(party["Keltur"], log, roller, this);
            Yoki = new Yoki(party["Alisa"]);
            Kurt = new Kurt(party["Urfin"]);
        }


        public void WeeklyLearning()
        {
            //boost nameless Instruction
            Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);



        }

        
    }
}