using NLog;

namespace RollerEngine.Logger
{
    public abstract class NLogLogger : BaseLogger
    {
        private readonly NLog.Logger _nLog;

        private static LogLevel Verbosity2LogLevel(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.Error:
                    return LogLevel.Fatal;
                case Verbosity.Warning:
                    return LogLevel.Error;
                case Verbosity.Critical:
                    return LogLevel.Warn;
                case Verbosity.Important:
                    return LogLevel.Info;
                case Verbosity.Normal:
                    return LogLevel.Debug;
                case Verbosity.Details:
                case Verbosity.Debug:
                    return LogLevel.Trace;
                default:
                    return LogLevel.Error;
            }
        }

        protected NLogLogger(string name, Verbosity verbosity) : base(verbosity)
        {
            _nLog = LogManager.GetLogger(name);
        }

        public override void Log(Verbosity verbosity, string record)
        {
            _nLog.Log(Verbosity2LogLevel(verbosity), ApplyFormat(record));
        }
    }

    class NLogFileLogger : NLogLogger
    {
        //should be synced with NLog.config xml
        private const string FILE_LOGGER_NAME = "FileLog";

        public NLogFileLogger(Verbosity verbosity) : base(FILE_LOGGER_NAME, verbosity)
        {
        }
    }

    class NLogConsoleLogger : NLogLogger
    {
        //should be synced with NLog.config xml
        private const string CONSOLE_LOGGER_NAME = "ConsoleLog";

        public NLogConsoleLogger(Verbosity verbosity) : base(CONSOLE_LOGGER_NAME, verbosity)
        {
        }
    }
}