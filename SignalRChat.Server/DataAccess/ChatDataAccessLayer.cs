using MongoDB.Bson;
using MongoDB.Driver;
using SignalRChat.Server.Interface;
using SignalRChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Server.DataAccess
{
    public class ChatDataAccessLayer : IChat
    {
        private ChatDBContext db;

        public ChatDataAccessLayer(ChatDBContext _db)
        {
            db = _db;
        }

        //To Get all messages     
        public List<Chat> GetAllMessages(string groupName)
        {
            try
            {
                return db.Chats.Find($"{{ GroupName: '{groupName}' }}").ToList();
            }
            catch
            {
                throw;
            }
        }

        //To Add new message record       
        public  void AddMessage(Chat chatData)
        {
            try
            {
                 db.Chats.InsertOneAsync(chatData);
            }
            catch
            {
                throw;
            }
        }


    }
}
