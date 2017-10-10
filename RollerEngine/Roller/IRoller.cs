using System.Collections.Generic;

namespace RollerEngine.Roller
{
    public interface IRoller
    {        
        List<int> Roll(int diceCount, int DC);
    }

    public class RollData
    {
        public int Successes { get; set; }
        public List<int> DiceResult { get; private set; }

        public RollData()
        {
            DiceResult = new List<int>();
        }

        public RollData(int successes, List<int> diceResult)
        {
            Successes = successes;
            DiceResult = diceResult;
        }
    }
}
