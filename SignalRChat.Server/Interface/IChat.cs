using SignalRChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Server.Interface
{
    public interface IChat
    {
        public List<Chat> GetAllMessages(string GroupName);
        public void AddMessage(Chat chat);
    }
}
