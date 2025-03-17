using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly UserService _mongoDbService;

        public NotificationController(MongoDbHelper dbHelper, UserService mongoDbService)
        {
            _notifications = dbHelper.GetDatabase().GetCollection<Notification>("Notifications");
            _mongoDbService = mongoDbService;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetAllNotifications()
        {
            var notifications = await _mongoDbService.GetCollectionDataAsync<Notification>("Notifications");
            return Ok(notifications);
        }

        // GET: api/notification/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, id);
            var notification = await _notifications.Find(filter).FirstOrDefaultAsync();

            if (notification == null)
                return NotFound("Notification not found.");
            return Ok(notification);
        }

        // POST: api/notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            if (notification == null)
                return BadRequest("Invalid notification data.");

            await _mongoDbService.CreateDocumentAsync("Notifications", notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
        }

        // PUT: api/notification/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, id);
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            var result = await _notifications.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound("Notification not found.");

            return NoContent();
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, id);
            var result = await _notifications.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                return NotFound("Notification not found.");

            return NoContent();
        }
    }
}
