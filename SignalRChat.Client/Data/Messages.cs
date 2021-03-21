using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Client.Data
{
    public static class Messages
    {
        public const string RECEIVE = "ReveiveMessage";
        public const string REGISTER = "Register";
        public const string SEND = "SendMessage";
        public const string USERS = "GetOnlineUsers";
        public const string DISCONNECTED = "UserDisconnected";
        public const string CONNECTED = "UserConnected";
        public const string MessageList = "MessageList";
        public static List<string> GroupNames = new List<string>() { "Group0", "Group1", "Group2", "Group3" };

    }
}
