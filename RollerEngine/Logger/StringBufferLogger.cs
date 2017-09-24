using System.Collections.Generic;
using System.Text;

namespace RollerEngine.Logger
{
    public class StringBufferLogger : ILogWrapper<StringBuilder>
    {
        private const int CAPACITY = 64 * 1024;

        //use LoggerFactory class
        private StringBufferLogger() { }

        public StringBuilder CreateChannelLogger(ActivityChannel channel)
        {
            return new StringBuilder(CAPACITY);
        }

        public void AppendInternalLog(StringBuilder logger, Verbosity verbosity, string record)
        {
            logger.Append(string.Format("{0}\n", record));
        }

        public string GetInternalLog(StringBuilder logger)
        {
            return logger.ToString();
        }

        public static class InnerLoggerFactory
        {
            public static BaseLogger<StringBufferLogger, StringBuilder> CreateStringBufferLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels)
            {
                return new BaseLogger<StringBufferLogger, StringBuilder>(minVerbosity, disabledChannels, new StringBufferLogger());
            }
        }
    }

    public static partial class LoggerFactory
    {
        public static BaseLogger<StringBufferLogger, StringBuilder> CreateStringBufferLogger(Verbosity minVerbosity, List<ActivityChannel> disabledChannels)
        {
            return StringBufferLogger.InnerLoggerFactory.CreateStringBufferLogger(minVerbosity, disabledChannels);
        }

        public static BaseLogger<StringBufferLogger, StringBuilder> CreateStringBufferLogger(Verbosity minVerbosity)
        {
            return CreateStringBufferLogger(minVerbosity, new List<ActivityChannel>());
        }

        public static BaseLogger<StringBufferLogger, StringBuilder> CreateStringBufferLogger()
        {
            return CreateStringBufferLogger(Verbosity.Debug);
        }
    }
}