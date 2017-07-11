using System;
using System.Threading;
using System.Collections.Concurrent;

namespace RolzOrgEnchancer
{
    //
    // provides Log method that can be called from any thread
    // everything except Log should be called from UI thread
    //
    class SafeLog
    {
        private ConcurrentQueue<string> log_queue;
        private Thread UI_thread;
        private IFormUpdate updater;

        public SafeLog(IFormUpdate _updater)
        {
            log_queue = new ConcurrentQueue<string>();
            UI_thread = Thread.CurrentThread;
            updater = _updater;
        }

        public void Log(string log_message)
        {
            if (Thread.CurrentThread != UI_thread)
            {
                log_queue.Enqueue(log_message);
            }
            else
            {
                updater.Log(log_message);
            }
        }

        public void ProcessLogQueue()
        {
            string log_message;
            if (log_queue.TryDequeue(out log_message))
            {
                updater.Log(log_message);
            }
        }

    }
}
