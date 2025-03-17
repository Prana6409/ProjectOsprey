using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileBackendTest1.Services; // Ensure this matches your Usernamespace
using MobileBackendTest1.Models;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _mongoDbService;
    private readonly IFileStorageService _fileStorageService;

    // Inject UserService
    public UsersController(UserService mongoDbService, IFileStorageService fileStorageService)
    {
        _mongoDbService = mongoDbService;
        _fileStorageService = fileStorageService;
    }

    // ✅ 2. GET ALL USERS
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _mongoDbService.GetUsersAsync();
        return Ok(users);
    }
    // Get user by username
    [HttpGet("by-username/{username}")]
    public async Task<ActionResult<User>> GetBusinessOwnerByUsername(string username)
    {
        var user = await _mongoDbService.GetUserByNameAsync(username);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }
        return Ok(user);
    }

    // Get all users By UserID
    [HttpGet("{UqID}")]
    public async Task<IActionResult> GetUser(string UqID)
    {
        var user1=await _mongoDbService.GetUserByUserID(UqID);
        if (user1 == null) 
        {
            return NotFound(new {Message="User not found."});
        }
        return Ok(user1);
    }
    // ✅ 3. CREATE A NEW USER
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest(new { success = false, message = "User data is required." });
        }

        if (string.IsNullOrEmpty(user.Username))
        {
            return BadRequest(new { success = false, message = "Username is required." });
        }

        if (string.IsNullOrEmpty(user.email))
        {
            return BadRequest(new { success = false, message = "Email is required." });
        }

        // Check if the name exists in the Users collection
        var existingUserByName = await _mongoDbService.GetUserByNameAsync(user.Username);
        if (existingUserByName != null)
        {
            return Conflict(new { success = false, message = "Username is already taken." });
        }

        // Check if the name exists in other collections (Sportsman, BusinessOwner, Entertainer)
        var nameExistsInOtherCollections = await _mongoDbService.IsNameExistsInOtherCollectionsAsync(user.Username);
        if (nameExistsInOtherCollections)
        {
            return Conflict(new { success = false, message = "Username is already taken in another collection." });
        }

        // Check if the email exists in the Users collection
        var existingUserByEmail = await _mongoDbService.GetUserByEmailAsync(user.email);
        if (existingUserByEmail != null)
        {
            return Conflict(new { success = false, message = "Email is already registered." });
        }

        // Check if the email exists in other collections (Sportsman, BusinessOwner, Entertainer)
        var emailExistsInOtherCollections = await _mongoDbService.IsEmailExistsInOtherCollectionsAsync(user.email);
        if (emailExistsInOtherCollections)
        {
            return Conflict(new { success = false, message = "Email is already registered in another collection." });
        }

        // Password Validation
        if (!IsValidPassword(user.password))
        {
            return BadRequest(new
            {
                success = false,
                message = "Password must be at least 8 characters long, contain an uppercase letter, a lowercase letter, a number, and a special character (@$!%*?&)."
            });
        }

        // Generate a JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("your_32_byte_long_secret_key_here_1234567890"); // Replace with a secure key
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.email)
            }),
            Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Save the user to the database
        await _mongoDbService.CreateUserAsync(user);

        // Return the token and user details
        return Ok(new
        {
            success = true,
            message = "User created successfully.",
            token = tokenString,
            user = new
            {
                id = user.Id,
                username = user.Username,
                email = user.email,
                address = user.address,
                dateofbirth = user.dateofbirth,
                country = user.country
            }
        });
    }

    // ✅ Password Validation Helper Method
    private bool IsValidPassword(string password)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        return Regex.IsMatch(password, pattern);
    }

    // ✅ 4. UPDATE USER DATA
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateUser(string Id, [FromBody] User updatedUser, IFormFile file)
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
                updatedUser.password = _mongoDbService.HashPassword(updatedUser.password);
            }


            // Update the user in the database
            await _mongoDbService.UpdateUserAsync(Id, updatedUser,file);

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
                    address = updatedUser.address,
                    dateofbirth = updatedUser.dateofbirth,
                    country = updatedUser.country,
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
    [HttpPost("{Id}/upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(int Id, IFormFile file)
    {
        try
        {
            var profilePicturePath = await _mongoDbService.UploadOrUpdateProfilePictureAsync(Id, file);
            return Ok(new { Message = "Profile picture uploaded successfully.", Path = profilePicturePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error uploading profile picture.", Error = ex.Message });
        }
    }

    [HttpGet("{UserId}/profile-picture")]
    public async Task<IActionResult> GetProfilePicture(int userId)
    {
        try
        {
            var imageBytes = await _mongoDbService.GetProfilePictureAsync(userId);
            return File(imageBytes, "image/jpeg"); // Adjust MIME type based on file type
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving profile picture.", Error = ex.Message });
        }
    }

    [HttpDelete("{Id}/profile-picture")]
    public async Task<IActionResult> DeleteProfilePicture(int Id)
    {
        try
        {
            var isDeleted = await _mongoDbService.DeleteProfilePictureAsync(Id);
            return Ok(new { Message = "Profile picture deleted successfully.", IsDeleted = isDeleted });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error deleting profile picture.", Error = ex.Message });
        }
    }


}