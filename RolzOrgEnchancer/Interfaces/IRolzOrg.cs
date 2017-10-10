namespace RolzOrgEnchancer.Interfaces
{
    //
    // UI thread only (except RoomEntered)
    // Rolz.Org interface for javascript
    //
    internal interface IRolzOrg
    {
        void JoinRoom(string roomName);

        bool RoomEntered();

        void SendMessage(string message);
    }
}
