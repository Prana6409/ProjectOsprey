using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MobileBackendTest1.Models
{
    public class BusinessOwner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UqId")]
        public string UqID { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("BusinessOwnerID")]
        public int BusinessOwnerID { get; set; }

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

        [BsonElement("companyname")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("IDcreateddate")]
        public DateTime IDCreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("headcountry")]
        public string HeadCountry { get; set; } = string.Empty;

        [BsonElement("password")]
        public string password { get; set; } = string.Empty;

        [BsonElement("website")]
        public string website {  get; set; } = string.Empty;
        
        [BsonElement("sponsorshipInterest")]
        public string sponsorshipInterest {  get; set; } = string.Empty;

        [BsonElement("Role")]
        public string Role { get; set; } = "BusinessOwner";

    }
}