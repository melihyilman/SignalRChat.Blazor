using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace SignalRChat.Shared.Models
{
    public class ChatDBContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        public ChatDBContext()
        {
            var client = new MongoClient("mongodb+srv://melihyilman:3295377@cluster0.mjv2l.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            _mongoDatabase = client.GetDatabase("Chats");
        }
       
        public IMongoCollection<Chat> Chats
        {
            get
            {
                return _mongoDatabase.GetCollection<Chat>("Chats");
            }
        }
    }

}
