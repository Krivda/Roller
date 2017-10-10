namespace RollerEngine.Logger
{
    public enum Verbosity
    {
        Error           = 6,
        Warning         = 5,
        Critical        = 4,
        Important       = 3,
        Normal          = 2,
        Details         = 1,
        Debug           = 0
    }

    public interface IRollLogger
    {
        void Log(Verbosity verbosity, string record);
        Verbosity MinVerbosity { get; set; }
        int Week { get; set; }
    }

}