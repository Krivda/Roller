namespace RollerEngine.Rolls
{
    public interface ILogger
    {
        void Log(Verbosity verbosity, string record);
    }

    public enum Verbosity
    {
        Warning, Important, Details, Debug
    }
}