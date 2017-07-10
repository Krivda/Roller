using System;
using System.Text;

namespace RollerEngine.Rolls
{
    public interface ILogger
    {
        void Log(string record);
        string GetLog();
    }

    class StringBufferLogger : ILogger
    {
        StringBuilder buf = new StringBuilder(15000);

        public void Log(string record)
        {
            buf.Append(string.Format("{0}\n", record));
        }


        public string GetLog()
        {
            return buf.ToString();
        }
    }

    
}