using System;

namespace RolzOrgEnchancer
{
    interface IRolzOrg
    {
        void JoinRoom(string room_name);
        bool RoomEntered();
        void SendMessage(string message);
        void SendSystemMessage(string message);
    }
}
