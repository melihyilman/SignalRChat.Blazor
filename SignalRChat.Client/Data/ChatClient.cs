using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRChat.Client.Data
{
    public class ChatClient : IAsyncDisposable
    {
        public const string HubUri = "/chathub";
        readonly NavigationManager navigationManager;
        HubConnection hubConnection;
        readonly string userName;
        readonly string groupName;
        bool started;
        public event MessageReceivedEventHandler MessageReceived;
        public event OnlineUsersEventHandler OnlineUsers;
        public event MessageListEventHandler MessageList;
        public event UserConnectedEventHandler UserConnected;
        public event UserDisconnectedEventHandler UserDisconnected;
        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
        public delegate void OnlineUsersEventHandler(object sender, List<string> userList);
        public delegate void UserConnectedEventHandler(object sender, string userName);
        public delegate void UserDisconnectedEventHandler(object sender,string userName);
        public delegate void MessageListEventHandler(object sender,List<Chat> messageList);

        public ChatClient(string groupName,string userName, NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
            this.userName = userName;
            this.groupName = groupName;
        }
        public async Task StartAsync()
        {
            if (!started)
            {
                hubConnection = new HubConnectionBuilder()
                                    .WithUrl(navigationManager.ToAbsoluteUri(HubUri))
                                    .Build();
                hubConnection.On<string, string>(Messages.RECEIVE, (userName, message) =>
                {
                    ReceiveMessageHandler(userName, message);
                });
                hubConnection.On<List<string>>(Messages.USERS, (users) =>
                {
                    ReceiveOnlineUsersHandler(users);
                });
                hubConnection.On<string>(Messages.CONNECTED, (userName) =>
                {
                    UserConnectedHandler(userName);
                });
                hubConnection.On<string>(Messages.DISCONNECTED, (userName) =>
                {
                    UserDisconnectedHandler(userName);
                });
                hubConnection.On<List<SignalRChat.Shared.Models.Chat>>(Messages.MessageList, (messageList) =>
                {
                    ReceiveMessageListHandler(messageList);
                });
                await hubConnection.StartAsync();
                Console.WriteLine("[CC]: Return");
                started = true;
                await hubConnection.SendAsync(Messages.REGISTER, groupName,userName);
                await hubConnection.SendAsync(Messages.USERS,groupName);
                await hubConnection.SendAsync(Messages.MessageList,groupName);
            }
        }
        public async Task StopAsync()
        {
            if (started)
            {
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                started = false;
            }
        }
        public async Task SendAsync(string message)
        {
            if (!started) throw new InvalidOperationException("Client isn't started..");

            await hubConnection.SendAsync(Messages.SEND, groupName,userName, message);
        }
        void ReceiveMessageHandler(string userName, string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(userName, message));
        }
        void ReceiveOnlineUsersHandler(List<string> userList)
        {
            OnlineUsers?.Invoke(this, userList);
        }
        void ReceiveMessageListHandler(List<SignalRChat.Shared.Models.Chat> userList)
        {
            MessageList?.Invoke(this, userList);
        }
        void UserConnectedHandler(string _userName)
        {
            UserConnected?.Invoke(this, _userName);
        }
        void UserDisconnectedHandler(string _userName)
        {
            UserDisconnected?.Invoke(this ,_userName);
        }
        public async ValueTask DisposeAsync()//this is buggy on blazor serverside. It's not firing on close the page
        {
            Console.WriteLine("[CC] : Disposing..");
            await StopAsync();
        }
    }
}
