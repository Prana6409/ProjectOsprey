using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MobileBackendTest1.Models
{
    public class Sportsman
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UqId")]
        public string UqID { get; set; } = Guid.NewGuid().ToString();


        [BsonElement("sportsmanID")]
        public int sportsmanID {  get; set; }

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;


        [BsonElement("profilepic")]
        public string ProfilePictureUrl { get; set; } // Add this property

        [BsonElement("email")]
        public string email { get; set; } = string.Empty;

        [BsonElement("contactnumber")]
        public long ContactNumber { get; set; }

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("intsport")]
        public string Sport { get; set; } = string.Empty;

        [BsonElement("Team")]
        public string? Team { get; set; }

        [BsonElement("dateofbirth")]
        public string DateOfBirth { get; set; } = string.Empty;

        [BsonElement("IDcreateddate")]
        public DateTime IDCreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;

        [BsonElement("password")]
        public string password { get; set; } = string.Empty;

        [BsonElement("Role")]
        public string Role { get; set; } = "Sportsman";

       
    
    }
}