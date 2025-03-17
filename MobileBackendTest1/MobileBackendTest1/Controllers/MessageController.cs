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
    public class MessageController : ControllerBase
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly UserService _mongoDbService;

        public MessageController(MongoDbHelper dbHelper, UserService mongoDbService)
        {
            _messages = dbHelper.GetDatabase().GetCollection<Message>("Messages");
            _mongoDbService = mongoDbService;
        }

        // GET: api/message
        [HttpGet]
        public async Task<ActionResult<List<Message>>> GetAllMessages()
        {
            var messages = await _mongoDbService.GetCollectionDataAsync<Message>("Messages");
            return Ok(messages);
        }

        // GET: api/message/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessageById(string id)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, id);
            var message = await _messages.Find(filter).FirstOrDefaultAsync();

            if (message == null)
                return NotFound("Message not found.");
            return Ok(message);
        }

        // GET: api/message/conversation?senderId=xxx&receiverId=yyy
        [HttpGet("conversation")]
        public async Task<ActionResult<List<Message>>> GetConversation([FromQuery] string senderId, [FromQuery] string receiverId)
        {
            var filter = Builders<Message>.Filter.Or(
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, senderId),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, receiverId)
                ),
                Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.SenderId, receiverId),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, senderId)
                )
            );

            var messages = await _messages.Find(filter).SortBy(m => m.Timestamp).ToListAsync();
            return Ok(messages);
        }

        // POST: api/message
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            if (message == null)
                return BadRequest("Invalid message data.");

            await _mongoDbService.CreateDocumentAsync("Messages", message);
            return CreatedAtAction(nameof(GetMessageById), new { id = message.Id }, message);
        }

        // DELETE: api/message/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, id);
            var result = await _messages.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                return NotFound("Message not found.");

            return NoContent();
        }
    }
}

