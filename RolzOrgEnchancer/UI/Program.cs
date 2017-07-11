using System;
using System.Windows.Forms;

namespace RolzOrgEnchancer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            safe_log = new SafeLog(form);
            Application.Run(form);
        }

        static public SafeLog safe_log;
        static public void Log(string log_message)
        {
            safe_log.Log(log_message);
        }

    }
}
