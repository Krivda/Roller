using System.Collections.Generic;
using RollerEngine.Extensions;

namespace RollerEngine.Logger
{
    public class BaseLogger<TWrapper, TLogger> : IBaseLogger
        where TWrapper : ILogWrapper<TLogger>
    {
        private readonly TWrapper _wrapper;
        private readonly TLogger _mainLogger;
        private readonly Dictionary<ActivityChannel, TLogger> _loggers;
        private readonly List<ActivityChannel> _disabledChannels;

        public Verbosity MinVerbosity { get; set; }
        public Verbosity SpecialVerbosityAlias { get; private set; }
        public int Week { get; set; }

        //use LoggerFactory class, all TImpl must have private constructor
        public BaseLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels, TWrapper wrapper)
        {
            _wrapper = wrapper;
            _loggers = new Dictionary<ActivityChannel, TLogger> { { ActivityChannel.Main, _mainLogger = _wrapper.CreateChannelLogger(ActivityChannel.Main) } };
            MinVerbosity = minVerbosity;
            _disabledChannels = disabledChannels;
            Week = -1;
            TreatSpecialVerbosityAs(Verbosity.Debug);
        }

        public void TreatSpecialVerbosityAs(Verbosity verbosity)
        {
            SpecialVerbosityAlias = verbosity;
        }

        public void Log(Verbosity verbosity, ActivityChannel channel, string record)
        {
            if (verbosity == Verbosity.Special) verbosity = SpecialVerbosityAlias;
            if (verbosity < MinVerbosity) return;
            if (_disabledChannels.Contains(channel)) return;
            var msg = record;
            if (Week != -1 ) msg = string.Format("W{0}: {1}", Week, record);
            _wrapper.AppendInternalLog(_mainLogger, verbosity, msg);
            if (channel == ActivityChannel.Main) return;

            TLogger logger;
            if (!_loggers.TryGetValue(channel, out logger)) _loggers.Add(channel, logger = _wrapper.CreateChannelLogger(channel));
            _wrapper.AppendInternalLog(logger, verbosity, record);
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