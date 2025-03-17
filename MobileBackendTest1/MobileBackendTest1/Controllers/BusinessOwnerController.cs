using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessOwnersController : ControllerBase
    {
        private readonly BusinessOwnerService _bownerService;

        public BusinessOwnersController(BusinessOwnerService bownerService)
        {
            _bownerService = bownerService;
        }

        // Get all business owners
        [HttpGet]
        public async Task<ActionResult<List<BusinessOwner>>> GetBusinessOwners()
        {
            var businessOwners = await _bownerService.GetBusinessOwnersAsync();
            return Ok(businessOwners);
        }

        // Get all users By UserID
        [HttpGet("{UqID}")]
        public async Task<IActionResult> GetUser(string UqID)
        {
            var user1 = await _bownerService.GetUserByUserID(UqID);
            if (user1 == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            return Ok(user1);
        }

        // Get a business owner by username
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<BusinessOwner>> GetBusinessOwnerByUsername(string username)
        {
            var businessOwner = await _bownerService.GetBusinessOwnerByUsernameAsync(username);
            if (businessOwner == null)
            {
                return NotFound(new { message = "Business owner not found." });
            }
            return Ok(businessOwner);
        }

        // Create a new business owner
        [HttpPost]
        public async Task<IActionResult> CreateBusinessOwner([FromBody] BusinessOwner businessOwner)
        {
            if (businessOwner == null)
            {
                return BadRequest(new { message = "Business owner data is required." });
            }

            if (string.IsNullOrEmpty(businessOwner.Username))
            {
                return BadRequest(new { message = "Username is required." });
            }

            if (string.IsNullOrEmpty(businessOwner.email))
            {
                return BadRequest(new { message = "Email is required." });
            }

            // Check if the username exists in the BusinessOwner collection
            var existingOwnerByUsername = await _bownerService.GetBusinessOwnerByUsernameAsync(businessOwner.Username);
            if (existingOwnerByUsername != null)
            {
                return Conflict(new { message = "Username is already taken." });
            }

            // Check if the username exists in other collections (Users, Sportsman, Entertainer)
            var usernameExistsInOtherCollections = await _bownerService.IsUsernameExistsInOtherCollectionsAsync(businessOwner.Username);
            if (usernameExistsInOtherCollections)
            {
                return Conflict(new { message = "Username is already taken in another collection." });
            }

            // Check if the email exists in the BusinessOwner collection
            var existingOwnerByEmail = await _bownerService.GetBusinessOwnerByEmailAsync(businessOwner.email);
            if (existingOwnerByEmail != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // Check if the email exists in other collections (Users, Sportsman, Entertainer)
            var emailExistsInOtherCollections = await _bownerService.IsEmailExistsInOtherCollectionsAsync(businessOwner.email);
            if (emailExistsInOtherCollections)
            {
                return Conflict(new { message = "Email is already registered in another collection." });
            }

            // Create the business owner
            await _bownerService.CreateBusinessOwnerAsync(businessOwner);

            return Ok(new
            {
                success = true,
                message = "Business owner created successfully.",
                businessOwner = new
                {
                    id = businessOwner.Id,
                    username = businessOwner.Username,
                    email = businessOwner.email,
                    contactnumber = businessOwner.ContactNumber,
                    address = businessOwner.Address,
                    companyname = businessOwner.CompanyName,
                    headcountry = businessOwner.HeadCountry,
                    website = businessOwner.website
                }
            });
        }

        // Update business owner data
        [HttpPut("{UqID}")]
        public async Task<IActionResult> UpdateUser(string UqID, [FromBody] BusinessOwner updatedUser, IFormFile file)
        {
            if (updatedUser == null)
            {
                return BadRequest(new { success = false, message = "User data is required." });
            }

            try
            {
                // Validate file if provided
                if (file != null && file.Length > 0)
                {
                    if (!file.ContentType.StartsWith("image/"))
                    {
                        return BadRequest(new { success = false, message = "Only image files are allowed." });
                    }

                    if (file.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        return BadRequest(new { success = false, message = "File size must be less than 5MB." });
                    }
                }
                // Hash the password if it's being updated
                if (!string.IsNullOrEmpty(updatedUser.password))
                {
                    updatedUser.password = _bownerService.HashPassword(updatedUser.password);
                }
                // Update the user in the database
                await _bownerService.UpdateUserAsync(UqID, updatedUser, file);

                // Return success response
                return Ok(new
                {
                    success = true,
                    message = "User updated successfully.",
                    user = new
                    {
                        
                        username = updatedUser.Username,
                        email = updatedUser.email,
                        address = updatedUser.Address,
                        contactnumber = updatedUser.ContactNumber,
                        companyname = updatedUser.CompanyName,
                        headcountry = updatedUser.HeadCountry,
                        website = updatedUser.website,
                        profilepicture = updatedUser.ProfilePictureUrl
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{UqID}/upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(string UqID, IFormFile file)
        {
            try
            {
                var profilePicturePath = await _bownerService.UploadOrUpdateProfilePictureAsync(UqID, file);
                return Ok(new { Message = "Profile picture uploaded successfully.", Path = profilePicturePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error uploading profile picture.", Error = ex.Message });
            }
        }

        [HttpGet("{UqID}/profile-picture")]
        public async Task<IActionResult> GetProfilePicture(string UqID)
        {
            try
            {
                var imageBytes = await _bownerService.GetProfilePictureAsync(UqID);
                return File(imageBytes, "image/jpeg"); // Adjust MIME type based on file type
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving profile picture.", Error = ex.Message });
            }
        }

        [HttpDelete("{UqID}/profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture(string UqID)
        {
            try
            {
                var isDeleted = await _bownerService.DeleteProfilePictureAsync(UqID);
                return Ok(new { Message = "Profile picture deleted successfully.", IsDeleted = isDeleted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting profile picture.", Error = ex.Message });
            }
        }
    }
}