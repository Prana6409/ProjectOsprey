using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MobileBackendTest1.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UniqueId")]
        public string EUqID { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("UqId")]
        public string UqId { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

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

        [BsonElement("Title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("Location")]
        public string Location { get; set; } = string.Empty;

        [BsonElement("Description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("StartDate")]
        public DateTime StartDate {  get; set; } = new DateTime();


        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; } = new DateTime();

        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [BsonElement("MaxCapacity")]
        public int MaxCapacity { get; set; }

        // Add a list of multimedia content to make the event more engaging
        [BsonElement("Contents")]
        public List<EventContentItem> Contents { get; set; } = new List<EventContentItem>();
    }

    public class EventContentItem
    {
        [BsonElement("Type")]
        public string Type { get; set; } // e.g., "video", "image", "text", "gif"

        [BsonElement("Url")]
        public string Url { get; set; } // URL or file path (optional for text)

        [BsonElement("Text")]
        public string Text { get; set; } // Optional: For plain text or captions

        [BsonElement("Description")]
        public string Description { get; set; } // Optional: Description of the content

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Timestamp for when the content was added
    }
}