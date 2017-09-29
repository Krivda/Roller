using System.Collections.Generic;
using System.Linq;

namespace RollerEngine.Logger
{
    public sealed class CompositeLogger : IBaseLogger
    {
        private int _week;
        private Verbosity _minVerbosity;
        public IBaseLogger[] Loggers { get; private set; }

        public int Week
        {
            get { return _week; }
            set
            {
                _week = value;
                foreach (var logger in Loggers)
                {
                    logger.Week = _week;
                }
            }
        }

        public Verbosity MinVerbosity
        {
            get { return _minVerbosity; }
            set
            {
                _minVerbosity = value;
                foreach (var logger in Loggers)
                {
                    logger.MinVerbosity = _minVerbosity;
                }
            }
        }

        //use LoggerFactory class
        private CompositeLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, params IBaseLogger[] loggers)
        {
            Loggers = loggers;
            MinVerbosity = minVerbosity;
            DisableActivityChannels(disabledChannels);
            Week = -1;
        }

        public void Log(Verbosity verbosity, ActivityChannel channel, string record)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(verbosity, channel, record);
            }
        }

        public void EnableActivityChannels(List<ActivityChannel> channels)
        {
            foreach (var logger in Loggers)
            {
                logger.EnableActivityChannels(channels);
            }
        }

        public void DisableActivityChannels(List<ActivityChannel> channels)
        {
            foreach (var logger in Loggers)
            {
                logger.DisableActivityChannels(channels);
            }
        }

        public string GetMainLog()
        {
            return Loggers.Aggregate("", (current, logger) => current + logger.GetChannelLog(ActivityChannel.Main));
        }

        public string GetChannelLog(ActivityChannel channel)
        {
            return Loggers.Aggregate("", (current, logger) => current + logger.GetChannelLog(channel));
        }

        public static class InnerLoggerFactory
        {
            public static CompositeLogger CreateCompositeLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, params IBaseLogger[] loggers)
            {
                return new CompositeLogger(minVerbosity, disabledChannels, loggers);
            }

        }

    }

    public static partial class LoggerFactory
    {
        public static CompositeLogger CreateCompositeLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, params IBaseLogger[] loggers)
        {
            return CompositeLogger.InnerLoggerFactory.CreateCompositeLogger(minVerbosity, disabledChannels, loggers);
        }

        public static CompositeLogger CreateCompositeLogger(Verbosity minVerbosity, params IBaseLogger[] loggers)
        {
            return CreateCompositeLogger(minVerbosity, new List<ActivityChannel>());
        }

        public static CompositeLogger CreateCompositeLogger(params IBaseLogger[] loggers)
        {
            return CreateCompositeLogger(Verbosity.Debug);
        }

    }
}