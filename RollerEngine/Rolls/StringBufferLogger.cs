using System.Text;

namespace RollerEngine.Rolls
{
    public class StringBufferLogger : ILogger
    {
        StringBuilder buf = new StringBuilder(15000);

        public void Log(Verbosity verbosity, string record)
        {
            buf.Append(string.Format("{0}\n", record));
        }


        public string GetLog()
        {
            return buf.ToString();
        }


    }
}