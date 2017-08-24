using System;

namespace RollerEngine.Rolls
{
    public class CompositeLogger : ILogger
    {
        private ILogger[] Loggers { get; set; }

        public CompositeLogger(params ILogger[] loggers)
        {
            Loggers = loggers;
        }


        public void Log(Verbosity verbosity, string record)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(verbosity, record);
            }
        }

        public string GetLog()
        {
            throw new NotImplementedException();
        }


    }
}