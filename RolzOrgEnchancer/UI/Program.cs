using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;

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
            main_thread = Thread.CurrentThread;
            log_queue = new ConcurrentQueue<string>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            logger = form;
            Application.Run(form);
        }

        static private Thread main_thread;
        static private ConcurrentQueue<string> log_queue;

        static public ILogger logger;

        static public void Log(string log_message) 
        {
            if (Thread.CurrentThread != main_thread)
            {
                log_queue.Enqueue(log_message);
            }
            else
            {
                logger.Log(log_message);
            }
        }

        static public void ProcessLogQueue()
        {
            string log_message;
            if (log_queue.TryDequeue(out log_message))
            {
                logger.Log(log_message);
            }
        }

    }
}
