using System;
using System.Windows.Forms;

namespace RolzOrgEnchancer.UI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            SafeLog = new SafeLog(form);
            Application.Run(form);
        }

        public static SafeLog SafeLog;
        public static void Log(string logMessage)
        {
            SafeLog.LogOrEnqueue(logMessage);
        }

    }
}
