using NLog;

namespace RollerEngine.Rolls
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _nlog;

        public NLogLogger(Logger nlog)
        {
            _nlog = nlog;
        }

        public void Log(Verbosity verbosity, string record)
        {

            LogLevel level;

            switch (verbosity)
            {
                case Verbosity.Warning:
                    level = LogLevel.Warn;
                    break;
                case Verbosity.Important:
                case Verbosity.Details:
                    level = LogLevel.Info;
                    break;
                case Verbosity.Debug:
                    level = LogLevel.Debug;
                    break;
                default:
                    level = LogLevel.Error;
                    break;
            }



            _nlog.Log(level, record);
        }

        public string GetLog()
        {
            throw new System.NotImplementedException();
        }
    }
}