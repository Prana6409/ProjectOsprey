using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MobileBackendTest1.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UqId")]
        public string UqID { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("UserID")]
        public int UserID { get; set; }

        [BsonElement("profilepic")]
        public string ProfilePictureUrl { get; set; } // Add this property

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        public string email { get; set; } = string.Empty;

        [BsonElement("address")]
        public string address { get; set; } = string.Empty;

        [BsonElement("contactnb")]
        public long number { get; set; }


        [BsonElement("dateofbirth")]

        public DateOnly dateofbirth { get; set; } = new DateOnly();

        [BsonElement("IDcreateddate")]

        public DateTime createddate { get; set; } = DateTime.UtcNow;

        [BsonElement("country")]

        public string country { get; set; } = string.Empty;

        [BsonElement("password")]
        public required string password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; } = "Users";

    }
}
