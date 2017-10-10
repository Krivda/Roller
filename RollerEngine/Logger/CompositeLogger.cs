using System.Collections.Generic;

namespace RollerEngine.Logger
{
    public sealed class CompositeLogger : BaseLogger
    {
        private Verbosity _minVerbosity;
        private int _week;

        public override Verbosity MinVerbosity
        {
            get { return _minVerbosity; }
            set
            {
                _minVerbosity = value;
                Loggers.ForEach(logger => logger.MinVerbosity = value);
            }
        }

        public override int Week
        {
            get { return _week; }
            set
            {
                _week = value;
                Loggers.ForEach(logger => logger.Week = value);
            }
        }

        public List<IRollLogger> Loggers { get; private set; }

        private CompositeLogger() : base(Verbosity.Debug)
        {
            Loggers = new List<IRollLogger>();
        }
       
        public override void Log(Verbosity verbosity, string record)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(verbosity, record);
            }
        }

        public void AddLogger(IRollLogger logger)
        {
            Loggers.Add(logger);
        }

        public static CompositeLogger InitLogging(Verbosity? consoleVerbosity, Verbosity? fileVerbosity, Verbosity? stringBuilderVerbosity, IRollLogger webLogger)
        {
            CompositeLogger logger = new CompositeLogger();

            if (stringBuilderVerbosity != null)
            {
                logger.AddLogger(new StringBufferLogger(stringBuilderVerbosity.Value));
            }

            if (consoleVerbosity != null)
            {
                logger.AddLogger(new NLogConsoleLogger(consoleVerbosity.Value));
            }

            if (fileVerbosity != null)
            {
                logger.AddLogger(new NLogFileLogger(fileVerbosity.Value));
            }

            if (webLogger != null)
            {
                logger.AddLogger(webLogger);
            }

            return logger;
        }

    }
}