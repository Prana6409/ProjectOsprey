using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntertainerController : ControllerBase
    {
        private readonly EntertainerService _entertainerService;

        public EntertainerController(EntertainerService entertainerService)
        {
            _entertainerService = entertainerService;
        }

        // Get all entertainers
        [HttpGet]
        public async Task<ActionResult<List<Entertainer>>> GetEntertainers()
        {
            var entertainers = await _entertainerService.GetEntertainersAsync();
            return Ok(entertainers);
        }

        // Get an entertainer by username
        [HttpGet("{username}")]
        public async Task<ActionResult<Entertainer>> GetEntertainerByUsername(string username)
        {
            var entertainer = await _entertainerService.GetEntertainerByUsernameAsync(username);
            if (entertainer == null)
            {
                return NotFound(new { message = "Entertainer not found." });
            }
            return Ok(entertainer);
        }

        // Create a new entertainer
        [HttpPost]
        public async Task<IActionResult> CreateEntertainer([FromBody] Entertainer entertainer)
        {
            if (entertainer == null)
            {
                return BadRequest(new { message = "Entertainer data is required." });
            }

            // Validate required fields
            if (string.IsNullOrEmpty(entertainer.Username) || string.IsNullOrEmpty(entertainer.email) ||
                entertainer.ContactNumber == 0 || string.IsNullOrEmpty(entertainer.Address) ||
                string.IsNullOrEmpty(entertainer.Talent) || string.IsNullOrEmpty(entertainer.password))
            {
                return BadRequest(new { message = "All required fields must be provided." });
            }

            // Validate email format
            if (!IsValidEmail(entertainer.email))
            {
                return BadRequest(new { message = "Invalid email format." });
            }

            // Validate password strength
            if (!IsValidPassword(entertainer.password))
            {
                return BadRequest(new
                {
                    message = "Password must be at least 8 characters long, contain an uppercase letter, a lowercase letter, a number, and a special character (@$!%*?&)."
                });
            }

            // Check if the username exists in the Entertainer collection
            var existingEntertainerByUsername = await _entertainerService.GetEntertainerByUsernameAsync(entertainer.Username);
            if (existingEntertainerByUsername != null)
            {
                return Conflict(new { message = "Username is already taken." });
            }

            // Check if the username exists in other collections (Users, Sportsman, BusinessOwner)
            var usernameExistsInOtherCollections = await _entertainerService.IsUsernameExistsInOtherCollectionsAsync(entertainer.Username);
            if (usernameExistsInOtherCollections)
            {
                return Conflict(new { message = "Username is already taken in another collection." });
            }

            // Check if the email exists in the Entertainer collection
            var existingEntertainerByEmail = await _entertainerService.GetEntertainerByEmailAsync(entertainer.email);
            if (existingEntertainerByEmail != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // Check if the email exists in other collections (Users, Sportsman, BusinessOwner)
            var emailExistsInOtherCollections = await _entertainerService.IsEmailExistsInOtherCollectionsAsync(entertainer.email);
            if (emailExistsInOtherCollections)
            {
                return Conflict(new { message = "Email is already registered in another collection." });
            }

            // Create the entertainer
            await _entertainerService.CreateEntertainerAsync(entertainer);

            return Ok(new
            {
                success = true,
                message = "Entertainer created successfully.",
                entertainer = new
                {
                    id = entertainer.Id,
                    username = entertainer.Username,
                    email = entertainer.email,
                    contactnumber = entertainer.ContactNumber,
                    address = entertainer.Address,
                    talent = entertainer.Talent,
                    dateofbirth = entertainer.DateOfBirth,
                    country = entertainer.Country
                }
            });
        }

        // Validate email format
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        // Validate password strength
        private bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        // Update entertainer data
        [HttpPut("{UqID}")]
        public async Task<IActionResult> UpdateUser(string UqID, [FromBody] Entertainer updatedUser, IFormFile file)
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
                    updatedUser.password =_entertainerService.HashPassword(updatedUser.password);
                }
                // Update the user in the database
                await _entertainerService.UpdateUserAsync(UqID, updatedUser,file);

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
                        talent = updatedUser.Talent,
                        dateofbirth = updatedUser.DateOfBirth,
                        country = updatedUser.Country,
                        stagename = updatedUser.stagename,
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
                var profilePicturePath = await _entertainerService.UploadOrUpdateProfilePictureAsync(UqID, file);
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
                var imageBytes = await _entertainerService.GetProfilePictureAsync(UqID);
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
                var isDeleted = await _entertainerService.DeleteProfilePictureAsync(UqID);
                return Ok(new { Message = "Profile picture deleted successfully.", IsDeleted = isDeleted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting profile picture.", Error = ex.Message });
            }
        }

    }
}
