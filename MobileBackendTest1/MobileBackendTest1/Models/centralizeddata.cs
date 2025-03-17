using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackendTest1.Models
{
    public class centralizeddata
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("EventId")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("UserId")]
        public string UserID { get; set; } = string.Empty;

        [BsonElement("BusinessOwnerId")]
        public string BusinessOwnerID { get; set; } = string.Empty;

        [BsonElement("SportsmanId")]
        public string SportsmanID { get; set; } = string.Empty;

        [BsonElement("EntertainerId")]
        public string EntertainerID { get; set; } = string.Empty;

        [BsonElement("Role")]
        public string Role {  get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public required string Password { get; set; }
    }
}
