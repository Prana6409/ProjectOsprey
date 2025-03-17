using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MobileBackendTest1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace MobileBackendTest1.Services
{
    public class ContentService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Content> _contents;
        private readonly CounterService _counterService;
        private readonly ILogger<ContentService> _logger;
        private readonly IMongoCollection<User> _users;
        
        private readonly IMongoCollection<Sportsman> _sportsmanCollection;
        private readonly IMongoCollection<BusinessOwner> _businessOwnerCollection;
        private readonly IMongoCollection<Entertainer> _entertainerCollection;


        // Dictionary to map content types to allowed extensions
        private readonly Dictionary<string, string[]> _allowedExtensions = new Dictionary<string, string[]>
        {
            { "video", new[] { ".mp4", ".mov", ".avi" } },
            { "reel", new[] { ".mp4", ".mov" } },
            { "image", new[] { ".jpg", ".jpeg", ".png", ".gif" } },
            { "document", new[] { ".pdf", ".docx", ".txt" } },
            { "text", new string[] { } } // Text content doesn't require extensions
        };
        public ContentService(IMongoDatabase database, CounterService counterService, ILogger<ContentService> logger)
        {
            _contents = database.GetCollection<Content>("Content");
            _counterService = counterService;
            _logger = logger;
            _counterService = counterService;
            _users = database.GetCollection<User>("Users"); // Ensure this matches your collection name
            _sportsmanCollection = database.GetCollection<Sportsman>("Sportsman");
            _businessOwnerCollection = database.GetCollection<BusinessOwner>("BusinessOwner");
            _entertainerCollection = database.GetCollection<Entertainer>("Entertainer");
        }


        // Get all contents
        public async Task<List<Content>> GetAllContentsAsync()
        {
            return await _contents.Find(_ => true).ToListAsync();
        }

        // Get content by ID
        public async Task<Content> GetContentByIdAsync(string id)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.CUqID, id);
            return await _contents.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Content>> GetContentsByIroledAsync(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                throw new ArgumentException("User ID and Role are required.");

            var filter = Builders<Content>.Filter.And(
                Builders<Content>.Filter.Eq(c => c.UqId, userId),
                Builders<Content>.Filter.Eq(c => c.Role.ToLower(), role.ToLower())
            );

            return await _contents.Find(filter).ToListAsync();
        }
        // Create new content
        public async Task CreateContentAsync(Content content,string userId,string role)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                throw new ArgumentException("User ID and Role are required.");

            // Associate content with the user
            content.UqId = userId;
            content.Role = role;

            // Validate required fields for single content
            if (content.Items == null || !content.Items.Any())
            {
                if (string.IsNullOrEmpty(content.Title) || string.IsNullOrEmpty(content.TypeOfContent) || string.IsNullOrEmpty(content.Url))
                    throw new ArgumentException("Title, TypeOfContent, and Url are required for single content.");
            }
            else
            {
                // Validate required fields for mixed content
                if (string.IsNullOrEmpty(content.Title))
                    throw new ArgumentException("Title is required for mixed content.");
            }

            // Generate a new ContentId
            int newContentId = await _counterService.GetNextIdAsync("ContentID");
            content.ContentId = newContentId;

            // Set the created and updated timestamps
            content.CreatedDate = DateTime.UtcNow;

            // Handle content based on whether it's single or mixed
            if (content.Items == null || !content.Items.Any())
            {
                // Handle single content
                await HandleSingleContentAsync(content);
            }
            else
            {
                // Handle mixed content
                await HandleMixedContentAsync(content);
            }

            // Insert content into MongoDB
            await _contents.InsertOneAsync(content);
            _logger.LogInformation($"Content with ID {content.Id} created.");
        }

        // Handle single content
        private async Task HandleSingleContentAsync(Content content)
        {
            // Validate URL format
            if (!IsValidUrl(content.Url))
                throw new ArgumentException("Invalid URL format.");

            // Handle different content types
            await HandleContentByTypeAsync(content.TypeOfContent, content.Url);
        }

        // Handle mixed content
        private async Task HandleMixedContentAsync(Content content)
        {
            foreach (var item in content.Items)
            {
                if (string.IsNullOrEmpty(item.TypeOfContent) || (string.IsNullOrEmpty(item.Url) && item.TypeOfContent.ToLower() != "text"))
                    throw new ArgumentException("TypeOfContent and Url are required for each content item.");

                // Validate URL format (except for text content)
                if (item.TypeOfContent.ToLower() != "text" && !IsValidUrl(item.Url))
                    throw new ArgumentException($"Invalid URL format for item: {item.Url}");

                // Handle different content types
                await HandleContentByTypeAsync(item.TypeOfContent, item.Url);
            }
        }

        // Handle content by type (supports multiple extensions)
        private async Task HandleContentByTypeAsync(string typeOfContent, string url)
        {
            string contentType = typeOfContent.ToLower();

            if (!_allowedExtensions.ContainsKey(contentType))
                throw new ArgumentException($"Unsupported content type: {contentType}");

            // Skip extension validation for text content
            if (contentType == "text")
            {
                _logger.LogInformation("Processing text content...");
                await Task.Delay(1);
                return;
            }

            // Validate URL extension
            string[] allowedExtensions = _allowedExtensions[contentType];
            if (!IsValidExtension(url, allowedExtensions))
                throw new ArgumentException($"Invalid file extension for {contentType}. Allowed extensions are: {string.Join(", ", allowedExtensions)}");

            _logger.LogInformation($"Processing {contentType} content...");
            await Task.Delay(1);
        }

        // Validate URL format
        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
         
        // Validate file extension
        private bool IsValidExtension(string url, string[] allowedExtensions)
        {
            foreach (var ext in allowedExtensions)
            {
                if (url.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        // Delete content
        public async Task DeleteContentAsync(string id)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.CUqID, id);
            var result = await _contents.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                throw new InvalidOperationException("Content not found.");
        }
    }
}