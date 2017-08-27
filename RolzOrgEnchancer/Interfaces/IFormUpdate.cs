namespace RolzOrgEnchancer.Interfaces
{
    //
    // UI thread only
    // interace to update Form
    //
    internal interface IFormUpdate
    {
        void Log(string logMessage); 

        void LogRoomLog(string roomLog);

        void UpdateActionQueueDepth(int depth);

    }
}
