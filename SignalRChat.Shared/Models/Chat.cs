using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SignalRChat.Shared.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string SenderName { get; set; }
        public string SenderMessage { get; set; }
        public string GroupName { get; set; }
        public DateTime SendDate { get; set; }
    }
}
