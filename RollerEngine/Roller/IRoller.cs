namespace RollerEngine.Roller
{
    public interface IRoller
    {        
        int Roll(int diceCount, int DC, bool removeSuccessOnOnes, bool hasSpecialization, bool hasWill, string description);

    }
}
