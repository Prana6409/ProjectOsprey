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
    public class UserPartnershipController : ControllerBase
    {
        private readonly IMongoCollection<UserPartnership> _userPartnerships;
        private readonly UserService _mongoDbService;
        private readonly CounterService _counterService;

        public UserPartnershipController(MongoDbHelper dbHelper, UserService mongoDbService, CounterService counterService)
        {
            _userPartnerships = dbHelper.GetDatabase().GetCollection<UserPartnership>("UserPartnerships");
            _mongoDbService = mongoDbService;
            _counterService = counterService;
        }

        // Get all user partnerships
        [HttpGet]
        public async Task<ActionResult<List<UserPartnership>>> GetAllUserPartnerships()
        {
            var userPartnershipsList = await _mongoDbService.GetCollectionDataAsync<UserPartnership>("UserPartnerships");
            return Ok(userPartnershipsList);
        }

        // Get a user partnership by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPartnership>> GetUserPartnershipById(string id)
        {
            var filter = Builders<UserPartnership>.Filter.Eq(up => up.Id, id);
            var userPartnership = await _userPartnerships.Find(filter).FirstOrDefaultAsync();

            if (userPartnership == null)
                return NotFound("UserPartnership not found.");
            return Ok(userPartnership);
        }

        // Create a new user partnership
        [HttpPost]
        public async Task<IActionResult> CreateUserPartnership([FromBody] UserPartnership userPartnership)
        {
            if (userPartnership == null)
                return BadRequest("Invalid user-partnership data.");

            // Set a new ID for the user partnership
            userPartnership.UserPartnershipId = await _counterService.GetNextIdAsync("userPartnershipID");
            userPartnership.CreatedAt = DateTime.Now;

            await _mongoDbService.CreateDocumentAsync("UserPartnerships", userPartnership);
            return CreatedAtAction(nameof(GetUserPartnershipById), new { id = userPartnership.Id }, userPartnership);
        }

        // Update an existing user partnership
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserPartnership(string id, [FromBody] UserPartnership updatedUserPartnership)
        {
            if (updatedUserPartnership == null)
                return BadRequest("Invalid user-partnership data.");

            var filter = Builders<UserPartnership>.Filter.Eq(up => up.Id, id);
            var update = Builders<UserPartnership>.Update
                .Set(up => up.UserId, updatedUserPartnership.UserId)
                .Set(up => up.PartnershipId, updatedUserPartnership.PartnershipId)
                .Set(up => up.CreatedAt, DateTime.Now);

            var result = await _userPartnerships.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound("UserPartnership not found.");

            return NoContent();
        }

        // Delete a user partnership
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserPartnership(string id)
        {
            var filter = Builders<UserPartnership>.Filter.Eq(up => up.Id, id);
            var result = await _userPartnerships.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                return NotFound("UserPartnership not found.");

            return NoContent();
        }
    }
}
