using System;

namespace SignalRChat.Client.Data
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public MessageReceivedEventArgs(string userName,string message)
        {
            UserName = userName;
            Message = message;
        }
    }
}
