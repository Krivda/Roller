using System;
using System.Collections.Generic;
using NLog;

namespace RollerEngine.Logger
{
    public class NLogLogger : ILogWrapper<NLog.Logger>
    {
        private readonly NLog.Logger _nlog;

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
                case Verbosity.Normal:
                    return LogLevel.Info;
                case Verbosity.Details:
                    return LogLevel.Debug;
                case Verbosity.Debug:
                    return LogLevel.Trace;
                default:
                    return LogLevel.Error;
            }
        }

        //use LoggerFactory class
        private NLogLogger(NLog.Logger nlog)
        {
            _nlog = nlog;
        }

        public NLog.Logger CreateChannelLogger(ActivityChannel channel)
        {
            return LogManager.GetLogger(string.Format("Channel_{0}.{1}", Enum.GetName(typeof(ActivityChannel), channel), _nlog.Name));
        }

        public void AppendInternalLog(NLog.Logger logger, Verbosity verbosity, string record)
        {
            logger.Log(Verbosity2LogLevel(verbosity), record);
        }

        public string GetInternalLog(NLog.Logger logger)
        {
            return logger.ToString();
        }

        public static class InnerLoggerFactory
        {
            public static BaseLogger<NLogLogger, NLog.Logger> CreateNLogLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, NLog.Logger nlog)
            {
                return new BaseLogger<NLogLogger, NLog.Logger>(minVerbosity, disabledChannels, new NLogLogger(nlog));
            }
        }
    }

    public static partial class LoggerFactory
    {
        public static BaseLogger<NLogLogger, NLog.Logger> CreateNLogLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, NLog.Logger nlog)
        {
            return NLogLogger.InnerLoggerFactory.CreateNLogLogger(minVerbosity, disabledChannels, nlog);
        }

        public static BaseLogger<NLogLogger, NLog.Logger> CreateNLogLogger(Verbosity minVerbosity, NLog.Logger nlog)
        {
            return CreateNLogLogger(minVerbosity, new List<ActivityChannel>(), nlog);
        }

        public static BaseLogger<NLogLogger, NLog.Logger> CreateNLogLogger(NLog.Logger nlog)
        {
            return CreateNLogLogger(Verbosity.Debug, nlog);
        }
    }
}