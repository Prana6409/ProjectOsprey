using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MobileBackendTest1.Models
{
    public class Entertainer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UqId")]
        public string UqID { get; set; } = Guid.NewGuid().ToString();


        [BsonElement("EntertainerId")]
        public int EntertainerId { get; set; }

        [BsonElement("profilepic")]
        public string ProfilePictureUrl { get; set; } // Add this property

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        public string email { get; set; } = string.Empty;

        [BsonElement("contactnumber")]
        public long ContactNumber { get; set; }

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("stagename")]
        public string stagename { get; set; } = string.Empty;

        [BsonElement("talent")]
        public string Talent { get; set; } = string.Empty;

        [BsonElement("dateofbirth")]
        public string DateOfBirth { get; set; } = string.Empty;

        [BsonElement("IDcreateddate")]
        public DateTime IDCreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;

        [BsonElement("password")]
        public string password { get; set; } = string.Empty;

        [BsonElement("Role")]
        public string Role { get; set; } = "Entertainer";
    }
}