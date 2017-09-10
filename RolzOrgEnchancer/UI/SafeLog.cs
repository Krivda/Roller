using System.Collections.Concurrent;
using System.Threading;
using RolzOrgEnchancer.Interfaces;

namespace RolzOrgEnchancer.UI
{
    //
    // provides LogOrEnqueue method that can be called from any thread
    // everything except LogOrEnqueue should be called from UI thread
    //
    internal class SafeLog
    {
        private readonly ConcurrentQueue<string> _logQueue;
        private readonly Thread _uiThread;
        private readonly IFormUpdate _updater;

        public SafeLog(IFormUpdate updater)
        {
            _logQueue = new ConcurrentQueue<string>();
            _uiThread = Thread.CurrentThread;
            _updater = updater;
        }

        public void LogOrEnqueue(string logMessage)
        {
            if (Thread.CurrentThread != _uiThread) _logQueue.Enqueue(logMessage);
            else _updater.AddToLog(logMessage);
        }

        public void ProcessLogQueue()
        {
            string logMessage;
            while (_logQueue.TryDequeue(out logMessage)) _updater.AddToLog(logMessage);
        }

    }
}
