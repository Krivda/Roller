using System.Collections.Generic;
using RollerEngine.Extensions;

namespace RollerEngine.Logger
{
    public class BaseLogger<TWrapper, TLogger> : IBaseLogger
        where TWrapper : ILogWrapper<TLogger>
    {
        public Verbosity MinVerbosity { get; private set; }
        private readonly TWrapper _wrapper;
        private readonly TLogger _mainLogger;
        private readonly Dictionary<ActivityChannel, TLogger> _loggers;
        private readonly List<ActivityChannel> _disabledChannels;

        //use LoggerFactory class, all TImpl must have private constructor
        public BaseLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, TWrapper wrapper)
        {
            _wrapper = wrapper;
            _loggers = new Dictionary<ActivityChannel, TLogger> { { ActivityChannel.Main, _mainLogger = _wrapper.CreateChannelLogger(ActivityChannel.Main) } };
            MinVerbosity = minVerbosity;
            _disabledChannels = disabledChannels;
        }

        public void Log(Verbosity verbosity, ActivityChannel channel, string record)
        {
            if (verbosity < MinVerbosity) return;
            if (_disabledChannels.Contains(channel)) return;
            _wrapper.AppendInternalLog(_mainLogger, verbosity, record);
            if (channel == ActivityChannel.Main) return;

            TLogger logger;
            if (!_loggers.TryGetValue(channel, out logger)) _loggers.Add(channel, logger = _wrapper.CreateChannelLogger(channel));
            _wrapper.AppendInternalLog(logger, verbosity, record);
        }

        public void SetMinimalVerbosity(Verbosity verbosity)
        {
            MinVerbosity = verbosity;
        }

        public void EnableActivityChannels(List<ActivityChannel> channels)
        {
            ListExtension.RemoveElements(_disabledChannels, channels);
        }

        public void DisableActivityChannels(List<ActivityChannel> channels)
        {
            ListExtension.AddNewElements(_disabledChannels, channels);
        }

        public string GetMainLog()
        {
            return GetChannelLog(ActivityChannel.Main);
        }

        public string GetChannelLog(ActivityChannel channel)
        {
            TLogger log;
            return _loggers.TryGetValue(channel, out log) ? _wrapper.GetInternalLog(log) : string.Empty;
        }

    }
}