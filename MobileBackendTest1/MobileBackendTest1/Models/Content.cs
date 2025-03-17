using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MobileBackendTest1.Models
{
    public class Content
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UniqueId")]
        public string CUqID { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonElement("typeofcontent")]
        public string TypeOfContent { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("ContentId")]
        public int ContentId { get; set; }

        public List<ContentItem> Items { get; set; } // For mixed content

        [BsonElement("UqId")] public string UqId { get; set; }

        [BsonElement("Role")] public string Role { get; set; }
 
        [BsonElement("WorkshopId")]
        public int? WorkshopId { get; set; }

        [BsonElement("SportsmanId")]
        public string SportsmanId { get; set; } // Foreign key to Sportsman

        [BsonElement("EntertainerId")]
        public string EntertainerId { get; set; } // Foreign key to Entertainer

        [BsonElement("BusinessOwnerId")]
        public string BusinessOwnerId { get; set; } // Foreign key to BusinessOwner
    }
    public class ContentItem
    {
        public string TypeOfContent { get; set; } // e.g., "video", "image", "text"
        public string Url { get; set; } // URL or file path (optional for text)
        public string Text { get; set; } // Optional: For plain text or captions with emojis

        [BsonElement("CreatedDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Timestamp for when the content was added
    }
}