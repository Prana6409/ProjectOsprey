using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MobileBackendTest1.Models
{
    public class RSVPs
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("EventId")]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("UserId")]
        public string UserID{ get; set; } = string.Empty;

        [BsonElement("BusinessOwnerId")]
        public string BusinessOwnerID { get; set; } = string.Empty;

        [BsonElement("SportsmanId")]
        public string SportsmanID { get; set; } = string.Empty;

        [BsonElement("EntertainerId")]
        public string EntertainerID { get; set; } = string.Empty;

        [BsonElement("Status")]
        public string status { get; set; } = string.Empty;

        [BsonElement("MaxCapacity")]
        public int MaxCapacity { get; set; } 

        

        [BsonElement("ReservedDate")]
        public DateTime ReservedDate { get; set; } = DateTime.Now;
    }
}
