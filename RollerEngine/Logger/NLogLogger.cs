using NLog;

namespace RollerEngine.Logger
{
    public class NLogLogger : IRollLogger
    {
        public Verbosity MinVerbosity { get; private set; }
        private readonly NLog.Logger _nlog;

        public NLogLogger(NLog.Logger nlog)
        {
            _nlog = nlog;
        }

        public NLogLogger(NLog.Logger nlog, Verbosity minVerbosity)
        {
            MinVerbosity = minVerbosity;
            _nlog = nlog;
        }

        public void Log(Verbosity verbosity, string record)
        {
            if (verbosity >= MinVerbosity)
            {
                LogLevel level;
                switch (verbosity)
                {
                    case Verbosity.Error:
                        level = LogLevel.Error;
                        break;
                    case Verbosity.Warning:
                    case Verbosity.Critical:
                        level = LogLevel.Warn;
                        break;
                    case Verbosity.Important:
                        level = LogLevel.Info;
                        break;
                    case Verbosity.Details:
                        level = LogLevel.Debug;
                        break;
                    case Verbosity.Debug:
                        level = LogLevel.Trace;
                        break;
                    default:
                        level = LogLevel.Error;
                        break;
                }
                _nlog.Log(level, record);
            }
        }

    }
}