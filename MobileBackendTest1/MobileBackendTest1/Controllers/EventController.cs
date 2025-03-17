using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using Microsoft.Extensions.Logging;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(EventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        // GET: api/event
        [HttpGet]
        public async Task<ActionResult<List<Event>>> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all events.");
                return StatusCode(500, "An error occurred while fetching events.");
            }
        }

        // GET: api/event/{id}
        [HttpGet("{EUqID}")]
        public async Task<ActionResult<Event>> GetEventById(string EUqID)
        {
            try
            {
                var eventItem = await _eventService.GetEventByIdAsync(EUqID);
                if (eventItem == null)
                {
                    return NotFound();
                }
                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching event with ID {EUqID}.");
                return StatusCode(500, "An error occurred while fetching the event.");
            }
        }

        // POST: api/event
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem, [FromHeader] string uid, [FromHeader] string role)
        {
            if (eventItem == null)
            {
                return BadRequest("Event data is required.");
            }

            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(role))
            {
                return BadRequest("User ID (uid) and Role are required in the request headers.");
            }

            try
            {
                var createdEvent = await _eventService.CreateEventAsync(eventItem, uid, role);
                return CreatedAtAction(nameof(GetEventById), new { EUqID = createdEvent.EUqID}, createdEvent);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid input data.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event.");
                return StatusCode(500, "An error occurred while creating the event.");
            }
        }

        // PUT: api/event/{id}
        [HttpPut("{EUqID}")]
        public async Task<IActionResult> UpdateEvent(string EUqID, [FromBody] Event eventItem)
        {
            if (EUqID != eventItem.EUqID)
            {
                return BadRequest("ID in the URL does not match the ID in the event object.");
            }

            try
            {
                await _eventService.UpdateEventAsync(EUqID, eventItem);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid input data.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event with ID {EUqID}.");
                return StatusCode(500, "An error occurred while updating the event.");
            }
        }

        // DELETE: api/event/{id}
        [HttpDelete("{EUqID}")]
        public async Task<IActionResult> DeleteEvent(string EUqID)
        {
            try
            {
                await _eventService.DeleteEventAsync(EUqID);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event with ID {EUqID}.");
                return StatusCode(500, "An error occurred while deleting the event.");
            }
        }

        // GET: api/event/user/{userId}/{role}
        [HttpGet("user/{uqid}/{role}")]
        public async Task<ActionResult<List<Event>>> GetEventsByUserIdAndRole(string uqid, string role)
        {
            if (string.IsNullOrEmpty(uqid) || string.IsNullOrEmpty(role))
            {
                return BadRequest("User ID and Role are required.");
            }

            try
            {
                var events = await _eventService.GetEventsByUserIdAndRoleAsync(uqid, role);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching events for user {uqid} with role {role}.");
                return StatusCode(500, "An error occurred while fetching events.");
            }
        }
    }
}