using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;

namespace MobileBackendTest1.Controllers
{
    // Define the route for the API controller, mapping it to 'api/membership'
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        // Declare the private readonly field for accessing the MembershipService
        private readonly MembershipService _membershipService;

        // Constructor that injects the MembershipService into the controller
        public MembershipController(MembershipService membershipService)
        {
            _membershipService = membershipService; // Assign the injected service to the private field
        }

        // Endpoint to get all memberships. It returns a list of memberships.
        [HttpGet]
        public async Task<ActionResult<List<Membership>>> GetAll()
        {
            var memberships = await _membershipService.GetAllMembershipsAsync();
            return Ok(memberships);  // Calls the service to retrieve all memberships and returns them with status 200 OK
        }

        // Endpoint to get a specific membership by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Membership>> GetById(string id)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(id);  // Calls the service to retrieve the membership by ID
            if (membership == null) // If no membership found, return 404 Not Found
                return NotFound("Membership not found");
            return Ok(membership); // Otherwise, return the found membership with status 200 OK
        }

        // Endpoint to create a new membership
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Membership membership)
        {
            if (membership == null || string.IsNullOrEmpty(membership.UserId) || string.IsNullOrEmpty(membership.MembershipType))
            {
                return BadRequest(new { message = "Invalid membership data. UserId and MembershipType are required." });
            }

            // Additional validation (e.g., StartDate, EndDate, etc.) could be added here

            await _membershipService.CreateMembershipAsync(membership); // Calls the service to create the membership in the database
            return CreatedAtAction(nameof(GetById), new { id = membership.MembershipId }, membership);
            // Returns 201 Created with the location of the newly created membership, including the membership data
        }

        // Endpoint to update an existing membership by ID
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Membership membership)
        {
            var existingMembership = await _membershipService.GetMembershipByIdAsync(id);  // Checks if the membership exists
            if (existingMembership == null) // If no membership found, return 404 Not Found
                return NotFound();

            // If MembershipId is auto-generated, no need to manually assign it
            // membership.MembershipId = id; 

            membership.UpdatedAt = DateTime.UtcNow; // Set the updated timestamp

            await _membershipService.UpdateMembershipAsync(id, membership); // Calls the service to update the membership in the database
            return NoContent(); // Returns 204 No Content to indicate the update was successful
        }

        // Endpoint to delete a membership by ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var membership = await _membershipService.GetMembershipByIdAsync(id); // Checks if the membership exists
            if (membership == null) // If no membership found, return 404 Not Found
                return NotFound("Membership not found");

            await _membershipService.DeleteMembershipAsync(id); // Calls the service to delete the membership from the database
            return NoContent(); // Returns 204 No Content to indicate the deletion was successful
        }
    }
}
