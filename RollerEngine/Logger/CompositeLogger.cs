namespace RollerEngine.Logger
{
    public class CompositeLogger : IRollLogger
    {
        private readonly IRollLogger[] _loggers;

        public CompositeLogger(params IRollLogger[] loggers)
        {
            _loggers = loggers;
        }       

        public void Log(Verbosity verbosity, string record)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(verbosity, record);
            }
        }

    }
}