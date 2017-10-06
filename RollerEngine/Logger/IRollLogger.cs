using System.Collections.Generic;

namespace RollerEngine.Logger
{
    public enum ActivityChannel
    {
        Main,           //main channel, have copies of all messages
        Intermediate,   //intermediate rolls
        Boost,          //boosting
        Restoration,    //replenish of Gnosis/Willpower/Rage/HP etc.
        Gathering,      //herbalism etc
        Creation,       //creation & crafts
        TeachLearn,     //teaching & learning
        Rites           //rites & caern
    }

    public enum Verbosity
    {
        Special         = 7,
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
        void Log(Verbosity verbosity, ActivityChannel channel, string record);
        int  Week { get; set; }
    }

    public interface IBaseLogger : IRollLogger
    {
        void EnableActivityChannels(List<ActivityChannel> channels);
        void DisableActivityChannels(List<ActivityChannel> channels);
        string GetMainLog();
        string GetChannelLog(ActivityChannel channel);
        Verbosity MinVerbosity { get; set; }
    }

    public interface ILogWrapper<TLogger>
    {
        TLogger CreateChannelLogger(ActivityChannel channel);
        void AppendInternalLog(TLogger logger, Verbosity verbosity, string record);
        string GetInternalLog(TLogger logger);
    }
}