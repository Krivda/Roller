using System.Text;

namespace RollerEngine.Logger
{
    public class StringBufferLogger : IRollLogger
    {
        private readonly StringBuilder _buf = new StringBuilder(15000);

        public void Log(Verbosity verbosity, string record)
        {
            _buf.Append(string.Format("{0}\n", record));
        }

        public string GetLog()
        {
            return _buf.ToString();
        }

    }
}