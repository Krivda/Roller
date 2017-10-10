namespace RollerEngine.Roller
{
    public interface IRollAnalyzer
    {
        RollData Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill);
    }
}