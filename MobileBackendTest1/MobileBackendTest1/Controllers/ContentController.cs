using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly ContentService _contentService;
        private readonly ILogger<ContentService> _logger;
        public ContentController(ContentService contentService, ILogger<ContentService> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }

        // Get all contents
        [HttpGet]
        public async Task<ActionResult<List<Content>>> GetAllContents()
        {
            var contentList = await _contentService.GetAllContentsAsync();
            return Ok(contentList);
        }

        // Get content by ID
        [HttpGet("{CUqID}")]
        public async Task<ActionResult<Content>> GetContentById(string CUqID)
        {
            var content = await _contentService.GetContentByIdAsync(CUqID);
            if (content == null)
                return NotFound("Content not found.");
            return Ok(content);
        }
        //Get bu UID and role of users
        [HttpGet("user/{uqid}/{role}")]
        public async Task<ActionResult<List<Content>>> GetContentsByUserIdAndRole(string uqid, string role)
        {
            if (string.IsNullOrEmpty(uqid) || string.IsNullOrEmpty(role))
                return BadRequest("User ID and Role are required.");

            var contentList = await _contentService.GetContentsByIroledAsync(uqid, role);
            return Ok(contentList);
        }

        // Create new content
        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] Content content, string uqid, string role)
        {
            if (content == null)
                return BadRequest("Invalid content data.");

            if (string.IsNullOrEmpty(uqid) || string.IsNullOrEmpty(role))
                return BadRequest("User ID and Role are required.");

            try
            {
                await _contentService.CreateContentAsync(content, uqid, role);
                return CreatedAtAction(nameof(GetContentById), new { id = content.Id }, content);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating content.");
                return StatusCode(500, "An error occurred while creating content.");
            }
        }




        // Delete content
        [HttpDelete("{CUqID}")]
        public async Task<IActionResult> DeleteContent(string CUqID)
        {
            try
            {
                await _contentService.DeleteContentAsync(CUqID);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound("Content not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting content.");
                return StatusCode(500, "An error occurred while deleting content.");
            }
        }
    }
}
