namespace RollerEngine.Logger
{
    public interface IRollLogger
    {
        void Log(Verbosity verbosity, string record);
    }

    public enum Verbosity
    {
        Warning, Important, Details, Debug
    }
}