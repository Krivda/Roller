using System.Text;

namespace RollerEngine.Logger
{
    public class StringBufferLogger : BaseLogger
    {
        private readonly StringBuilder _logger;
        private const int CAPACITY = 64 * 1024;

        public StringBufferLogger(Verbosity verbosity) : base(verbosity)
        {
            _logger = new StringBuilder(CAPACITY);
        }

        public override void Log(Verbosity verbosity, string record)
        {
            _logger.Append(ApplyFormat(record));
        }
    }
}