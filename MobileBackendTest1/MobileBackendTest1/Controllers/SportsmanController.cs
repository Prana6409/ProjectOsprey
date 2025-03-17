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
    public class SportsmanController : ControllerBase
    {
        private readonly SportsmanService _sportsmanService;

        public SportsmanController(SportsmanService sportsmanService)
        {
            _sportsmanService = sportsmanService;
        }

        // Get all sportsmen
        [HttpGet]
        public async Task<ActionResult<List<Sportsman>>> GetSportsmen()
        {
            var sportsmen = await _sportsmanService.GetSportsmenAsync();
            return Ok(sportsmen);
        }

        // Get a sportsman by username
        [HttpGet("{username}")]
        public async Task<ActionResult<Sportsman>> GetSportsmanByUsername(string username)
        {
            var sportsman = await _sportsmanService.GetSportsmanByUsernameAsync(username);
            if (sportsman == null)
            {
                return NotFound(new { message = "Sportsman not found." });
            }
            return Ok(sportsman);
        }

        // Create a new sportsman
        [HttpPost]
        public async Task<IActionResult> CreateSportsman([FromBody] Sportsman sportsman)
        {
            if (sportsman == null)
            {
                return BadRequest(new { message = "Sportsman data is required." });
            }

            // Validate required fields
            if (string.IsNullOrEmpty(sportsman.Username) || string.IsNullOrEmpty(sportsman.email) ||
                sportsman.ContactNumber == 0 || string.IsNullOrEmpty(sportsman.Address) ||
                string.IsNullOrEmpty(sportsman.Sport) || string.IsNullOrEmpty(sportsman.password))
            {
                return BadRequest(new { message = "All required fields must be provided." });
            }

            // Validate email format
            if (!IsValidEmail(sportsman.email))
            {
                return BadRequest(new { message = "Invalid email format." });
            }

            // Validate password strength
            if (!IsValidPassword(sportsman.password))
            {
                return BadRequest(new
                {
                    message = "Password must be at least 8 characters long, contain an uppercase letter, a lowercase letter, a number, and a special character (@$!%*?&)."
                });
            }

            // Check if the username exists in the Sportsman collection
            var existingSportsmanByUsername = await _sportsmanService.GetSportsmanByUsernameAsync(sportsman.Username);
            if (existingSportsmanByUsername != null)
            {
                return Conflict(new { message = "Username is already taken." });
            }

            // Check if the username exists in other collections (Users, BusinessOwner, Entertainer)
            var usernameExistsInOtherCollections = await _sportsmanService.IsUsernameExistsInOtherCollectionsAsync(sportsman.Username);
            if (usernameExistsInOtherCollections)
            {
                return Conflict(new { message = "Username is already taken in another collection." });
            }

            // Check if the email exists in the Sportsman collection
            var existingSportsmanByEmail = await _sportsmanService.GetSportsmanByEmailAsync(sportsman.email);
            if (existingSportsmanByEmail != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // Check if the email exists in other collections (Users, BusinessOwner, Entertainer)
            var emailExistsInOtherCollections = await _sportsmanService.IsEmailExistsInOtherCollectionsAsync(sportsman.email);
            if (emailExistsInOtherCollections)
            {
                return Conflict(new { message = "Email is already registered in another collection." });
            }

            // Create the sportsman
            await _sportsmanService.CreateSportsmanAsync(sportsman);

            return Ok(new
            {
                success = true,
                message = "Sportsman created successfully.",
                sportsman = new
                {
                    id = sportsman.Id,
                    username = sportsman.Username,
                    email = sportsman.email,
                    contactnumber = sportsman.ContactNumber,
                    address = sportsman.Address,
                    sport = sportsman.Sport,
                    dateofbirth = sportsman.DateOfBirth,
                    country = sportsman.Country
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

        // Update sportsman data
        [HttpPut("{UqID}")]
        public async Task<IActionResult> UpdateUser(string UqID, [FromBody] Sportsman updatedUser, IFormFile file)
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
                    updatedUser.password = _sportsmanService.HashPassword(updatedUser.password);
                }
                // Update the user in the database
                await _sportsmanService.UpdateUserAsync(UqID, updatedUser,file);

                // Return success response
                return Ok(new
                {
                    success = true,
                    message = "User updated successfully.",
                    user = new
                    {
                        id = updatedUser.Id,
                        username = updatedUser.Username,
                        email = updatedUser.email,
                        address = updatedUser.Address,
                        contactnumber = updatedUser.ContactNumber,
                        sport = updatedUser.Sport,
                        dateofbirth = updatedUser.DateOfBirth,
                        country = updatedUser.Country,
                        team = updatedUser.Team
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
                var profilePicturePath = await _sportsmanService.UploadOrUpdateProfilePictureAsync(UqID, file);
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
                var imageBytes = await _sportsmanService.GetProfilePictureAsync(UqID);
                return File(imageBytes, "image/jpeg"); // Adjust MIME type based on file type
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving profile picture.", Error = ex.Message });
            }
        }

        [HttpDelete("{UqID}/profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture(string UqID )
        {
            try
            {
                var isDeleted = await _sportsmanService.DeleteProfilePictureAsync(UqID);
                return Ok(new { Message = "Profile picture deleted successfully.", IsDeleted = isDeleted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting profile picture.", Error = ex.Message });
            }
        }
    }
}