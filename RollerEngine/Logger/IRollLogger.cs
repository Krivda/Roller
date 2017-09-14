namespace RollerEngine.Logger
{
    public interface IRollLogger
    {
        void Log(Verbosity verbosity, string record);
    }

    public enum Verbosity
    {
        Error =5,
        Warning =4,
        Critical=3,
        Important=2,
        Details =1,
        Debug =0 
    }
}