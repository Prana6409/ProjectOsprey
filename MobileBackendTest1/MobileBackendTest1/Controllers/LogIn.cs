using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MobileBackendTest1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LogInService _loginService;

        public LoginController(LogInService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogIn loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.password))
            {
                return BadRequest(new { success = false, message = "Email and Password are required." });
            }

            // Authenticate the user
            var result = await _loginService.AuthenticateAsync(loginModel.Email, loginModel.password);

            if (result == null)
            {
                return Unauthorized(new { success = false, message = "Invalid email or password." });
            }

            // Generate a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_32_byte_long_secret_key_here_1234567890"); // Replace with a secure key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
                
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Role, result.Role)
            }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Return the token and user details
            return Ok(new
            {
                success = true,
                message = "Login successful.",
                token = tokenString,
                user = new
                {
                    id = result.Id,
                    uqid= result.Uqid,
                    email = result.Email,
                    role = result.Role
                }
            });
        }
    }
