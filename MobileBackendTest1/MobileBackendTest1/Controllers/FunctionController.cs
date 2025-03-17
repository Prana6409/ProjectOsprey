using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Services;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly FunctionService _functionservice;

        public FunctionController(FunctionService functionservice)
        {
            _functionservice = functionservice;   
        }

        [HttpGet("search-usernames")]
        public async Task<IActionResult> SearchUsernames(string query, bool exactMatch = false)
        {
            var results = await _functionservice.SearchUsernamesAsync(query, exactMatch);
            if (!results.Any())
            {
                return NotFound($"No results found for '{query}'.");
            }
            return Ok(results);
        }
        [HttpGet("user-profile/{Username}")]
        public async Task<IActionResult> GetUserProfile(string UqID)
        {
            var profile = await _functionservice.GetUserProfileAsync(UqID);
            if (profile == null)
            {
                return NotFound($"User with ID {UqID} not found.");
            }
            return Ok(profile);
        }
    }
}
