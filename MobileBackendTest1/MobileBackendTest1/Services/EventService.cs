using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MobileBackendTest1.Models;
using Microsoft.Extensions.Logging;

namespace MobileBackendTest1.Services
{
    public class EventService
    {
        private readonly IMongoCollection<Event> _events;
        private readonly CounterService _counterService;
        private readonly ILogger<EventService> _logger;


        public EventService(IMongoDatabase database, CounterService counterService, ILogger<EventService> logger)
        {
            _events = database.GetCollection<Event>("Events");
            _counterService = counterService;
            _logger = logger;
        }

        // Get all events
        public async Task<List<Event>> GetAllEventsAsync()
        {
            try
            {
                return await _events.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all events.");
                throw;
            }
        }

        // Get an event by ID
        public async Task<Event> GetEventByIdAsync(string id)
        {
            try
            {
                return await _events.Find(e => e.EUqID == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching event with ID {id}.");
                throw;
            }
        }

        // Create a new event
        public async Task<Event> CreateEventAsync(Event eventItem, string uid, string role)
        {
            if (eventItem == null)
                throw new ArgumentNullException(nameof(eventItem));

            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(role))
                throw new ArgumentException("User ID and Role are required.");

            try
            {
                // Assign uid and role to the event
                eventItem.UqId = uid;
                eventItem.Role = role;

                // Generate a unique EventId using CounterService
                int newEventId = await _counterService.GetNextIdAsync("EventID");
                eventItem.EventId = newEventId.ToString();

                // Infer and set the Type for each content item
                if (eventItem.Contents != null && eventItem.Contents.Count > 0)
                {
                    foreach (var content in eventItem.Contents)
                    {
                        content.Type = InferContentType(content);
                        ValidateEventContentItem(content);
                    }
                }

                await _events.InsertOneAsync(eventItem);
                _logger.LogInformation($"Event created with ID {eventItem.EUqID} and EventId {eventItem.EventId}.");
                return eventItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event.");
                throw;
            }
        }

        // Infer the content type based on Url or Text
        private string InferContentType(EventContentItem contentItem)
        {
            if (!string.IsNullOrEmpty(contentItem.Text))
            {
                return "text"; // If Text is provided, it's a text content
            }

            if (!string.IsNullOrEmpty(contentItem.Url))
            {
                // Infer type based on file extension
                string extension = System.IO.Path.GetExtension(contentItem.Url)?.ToLower();
                switch (extension)
                {
                    case ".mp4":
                    case ".mov":
                    case ".avi":
                        return "video"; // Video content
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                        return "image"; // Image content
                    default:
                        return "unknown"; // Unknown type (you can handle this as needed)
                }
            }

            throw new ArgumentException("Either Text or Url must be provided for the content item.");
        }

        // Validate an EventContentItem
        private void ValidateEventContentItem(EventContentItem contentItem)
        {
            if (contentItem == null)
                throw new ArgumentNullException(nameof(contentItem));

            if (string.IsNullOrEmpty(contentItem.Type))
                throw new ArgumentException("Content type is required.");

            if (contentItem.Type != "text" && string.IsNullOrEmpty(contentItem.Url))
                throw new ArgumentException("URL is required for non-text content.");

            if (contentItem.Type == "text" && string.IsNullOrEmpty(contentItem.Text))
                throw new ArgumentException("Text is required for text content.");
        }

        // Other methods (UpdateEventAsync, DeleteEventAsync, etc.) remain unchanged


        // Update an existing event
        public async Task UpdateEventAsync(string id, Event eventItem)
        {
            if (id != eventItem.EUqID)
                throw new ArgumentException("ID in the URL does not match the ID in the event object.");

            try
            {
                // Validate the event's contents (if any)
                if (eventItem.Contents != null && eventItem.Contents.Count > 0)
                {
                    foreach (var content in eventItem.Contents)
                    {
                        ValidateEventContentItem(content);
                    }
                }

                await _events.ReplaceOneAsync(e => e.EUqID == id, eventItem);
                _logger.LogInformation($"Event with ID {id} updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event with ID {id}.");
                throw;
            }
        }

        // Delete an event
        public async Task DeleteEventAsync(string id)
        {
            try
            {
                await _events.DeleteOneAsync(e => e.EUqID == id);
                _logger.LogInformation($"Event with ID {id} deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event with ID {id}.");
                throw;
            }
        }

        // Add content to an event
        public async Task AddContentToEventAsync(string eventId, EventContentItem contentItem)
        {
            if (string.IsNullOrEmpty(eventId))
                throw new ArgumentException("Event ID is required.");

            if (contentItem == null)
                throw new ArgumentNullException(nameof(contentItem));

            try
            {
                ValidateEventContentItem(contentItem);

                var filter = Builders<Event>.Filter.Eq(e => e.EUqID, eventId);
                var update = Builders<Event>.Update.Push(e => e.Contents, contentItem);

                await _events.UpdateOneAsync(filter, update);
                _logger.LogInformation($"Content added to event with ID {eventId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding content to event with ID {eventId}.");
                throw;
            }
        }

        // Remove content from an event
        public async Task RemoveContentFromEventAsync(string eventId, string contentItemId)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(contentItemId))
                throw new ArgumentException("Event ID and Content Item ID are required.");

            try
            {
                var filter = Builders<Event>.Filter.Eq(e => e.EUqID, eventId);
                var update = Builders<Event>.Update.PullFilter(e => e.Contents, c => c.Url == contentItemId);

                await _events.UpdateOneAsync(filter, update);
                _logger.LogInformation($"Content with ID {contentItemId} removed from event with ID {eventId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing content from event with ID {eventId}.");
                throw;
            }
        }

        // Get events by UserId and Role
        public async Task<List<Event>> GetEventsByUserIdAndRoleAsync(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                throw new ArgumentException("User ID and Role are required.");

            try
            {
                var filter = Builders<Event>.Filter.And(
                    Builders<Event>.Filter.Eq(e => e.UqId, userId),
                    Builders<Event>.Filter.Eq(e => e.Role.ToLower(), role.ToLower())
                );

                return await _events.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching events for user {userId} with role {role}.");
                throw;
            }
        }


    }
}