using System.Collections.Generic;

namespace RollerEngine.Roller
{
    public interface IRoller
    {        
        RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description);

    }

    public class RollData
    {
        public int Successes { get; set; }
        public List<int> DiceResult { get; private set; }

        public RollData()
        {
            DiceResult = new List<int>();
        }
    }
}
