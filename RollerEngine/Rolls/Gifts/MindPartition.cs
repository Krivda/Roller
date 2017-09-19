using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Gifts
{
    public class MindPartition : GiftRoll
    {
        private int _botchCount;
        public const string GiftName = "Mind partition";

        /*
         * System: The player spends one Willpower point and
         * rolls Gnosis. (Difficulty 6) For every success gained, the
         * player can add work on one extra extended action every
         * turn, one added per turn. No penalties are incurred on any
         * of the extended actions, but if just one botch is rolled, all
         * uncompleted tasks fail.
         */

        public MindPartition(IRollLogger log, IRoller roller)
            : base(GiftName, log, roller,
                new List<string>() { Build.RollableTraits.Gnosis },
                new List<string>(),
                null, Verbosity.Important)
        {
            
        }

        public int Roll(Build actor)
        {
            _botchCount = 0;
            int successes = base.Roll(actor, new List<Build>() { actor }, false, false);

            if (successes < 0)
            {
                _botchCount = 1;
                //reroll
                Log.Log(Verbosity, string.Format("{0} has botched {1} gift on {2} successes and is re-rolling it.", actor.Name, Name,successes));
                successes = base.Roll(actor, new List<Build>() { actor }, false, false);
            }

            if (successes > 0)
            {
                Log.Log(Verbosity, string.Format("{0} obtained {1} bonus extended actions for a scence from {2} gift.", actor.Name, successes, Name));
            } 
            else
            {
                Log.Log(Verbosity, string.Format("{0} didn't get bonus from {1}.", actor.Name, Name));
            }

            return successes;
        }

        protected override int OnBotch(int successes)
        {
            if (_botchCount == 0)
            {
                return successes;
            }
            //FUCKING FUCKED
            //todo: remove ALL success from number of pools equal to botch level?
            return base.OnBotch(successes);
        }
    }
}