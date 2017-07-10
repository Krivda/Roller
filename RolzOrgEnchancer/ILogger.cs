using System;

namespace RolzOrgEnchancer
{
    interface ILogger
    {
        void Log(string log_message); 
        void LogRoomLog(string room_log);
        void UpdateActionQueueDepth(int depth);
    }
}
