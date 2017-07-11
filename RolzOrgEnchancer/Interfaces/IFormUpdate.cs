using System;

namespace RolzOrgEnchancer
{
    //
    // UI thread only
    // interace to update Form
    //
    interface IFormUpdate
    {
        void Log(string log_message); 

        void LogRoomLog(string room_log);

        void UpdateActionQueueDepth(int depth);

    }
}
