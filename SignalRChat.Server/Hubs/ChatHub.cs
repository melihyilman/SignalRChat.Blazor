using Microsoft.AspNetCore.SignalR;
using SignalRChat.Client.Data;
using SignalRChat.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Server.Hubs
{
    public class UserLookup
    {
        public UserLookup(string groupName, string userName)
        {
            GroupName = groupName;
            UserName = userName;
        }
        public string UserName { get; set; }
        public string GroupName { get; set; }
    }
    public class ChatHub : Hub
    {
        static readonly Dictionary<string, UserLookup> userLookup = new Dictionary<string, UserLookup>();
        readonly ChatDataAccessLayer chatDataAccessLayer = new ChatDataAccessLayer(new Shared.Models.ChatDBContext());

        public async Task SendMessage(string groupName, string userName, string message)
        {
            await Clients.Group(groupName).SendAsync(Messages.RECEIVE, userName, message);
            chatDataAccessLayer.AddMessage(new Shared.Models.Chat() { GroupName = groupName, SenderName = userName, SenderMessage = message, SendDate = DateTime.Now });
        }
        public async Task Register(string groupName, string userName)
        {
            var currentId = Context.ConnectionId;
            if (!userLookup.ContainsKey(currentId))
            {
                userLookup.Add(currentId, new UserLookup(groupName, userName));
                await UserConnected(groupName, userName);
                await JoinRoom(groupName);
            }
        }
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
        public async Task GetOnlineUsers(string groupName)
        {
            var currentId = Context.ConnectionId;
            var users = userLookup.Where(x => x.Value != null && x.Value.GroupName == groupName).Select(x => x.Value.UserName).ToList();
            await Clients.Client(currentId).SendAsync(Messages.USERS, users);
        }
        public async Task MessageList(string groupName)
        {
            var currentId = Context.ConnectionId;
            var messageList = chatDataAccessLayer.GetAllMessages(groupName);
            await Clients.Client(currentId).SendAsync(Messages.MessageList, messageList);

        }

        public async Task UserConnected(string groupName, string userName)
        {
            await Clients.OthersInGroup(groupName).SendAsync(Messages.CONNECTED, userName);
        }
        public async Task UserDisonnected(string groupName, string _userName)
        {
            await Clients.OthersInGroup(groupName).SendAsync(Messages.DISCONNECTED, _userName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[!] user disconnected: {Context.ConnectionId} -> {exception?.Message}");
            var currentId = Context.ConnectionId;
            if (!userLookup.TryGetValue(currentId, out var user))
            {
                user.UserName = "![DisconnectedUser]";
            }
            userLookup.Remove(currentId);
            await UserDisonnected(user.GroupName, user.UserName);
            await LeaveRoom(user.GroupName);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
