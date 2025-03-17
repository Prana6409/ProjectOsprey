using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MobileBackendTest1.Models
{
    public class Membership
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("MembershipID")]
        public string MembershipId { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)] // Ensures it's stored as an ObjectId
        public string UserId { get; set; } = string.Empty;

        [BsonElement("membershipType")]
        public string MembershipType { get; set; } = string.Empty;

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
