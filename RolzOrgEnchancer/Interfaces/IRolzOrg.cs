using System;

namespace RolzOrgEnchancer
{
    //
    // UI thread only (except RoomEntered)
    // Rolz.Org interface for javascript
    //
    interface IRolzOrg
    {
        void JoinRoom(string room_name);

        bool RoomEntered();

        void SendMessage(string message);

        void SendSystemMessage(string message);

    }
}
