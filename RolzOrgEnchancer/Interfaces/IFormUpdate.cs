namespace RolzOrgEnchancer.Interfaces
{
    //
    // UI thread only
    // interace to update Form
    //
    internal interface IFormUpdate
    {
        void AddToLog(string logMessage);

        void UpdateRoomLog(string roomLog);

        void UpdateActionQueueDepth(int depth);

    }
}
