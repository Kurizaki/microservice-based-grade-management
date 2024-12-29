using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using authentification_service.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace authentification_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AUTHDB _context;                // Database context for user data
        private readonly IConfiguration _configuration;  // Access to application settings

        public AuthController(AUTHDB context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Registers a new user.
        /// POST: api/Auth/register
        /// </summary>
        /// <param name="request">User data (Username, Password)</param>
        /// <returns>HTTP 200 if successful, otherwise an error.</returns>
        [HttpPost("register/")]
        public IActionResult Register([FromBody] UserDTO request)
        {
            // Input Validation: Ensure username and password are not empty
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            // Check if the username already exists in the database
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists." });
            }

            // Hash the user's password using BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create a new User entity
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            // Save the new user to the database
            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error saving user to the database.",
                    error = ex.Message
                });
            }

            return Ok(new { message = "Registration successful.", username = newUser.Username });
        }

        /// <summary>
        /// Logs in an existing user.
        /// POST: api/Auth/login
        /// </summary>
        /// <param name="request">User data (Username, Password)</param>
        /// <returns>JWT token if successful, otherwise unauthorized.</returns>
        [HttpPost("login/")]
        public IActionResult Login([FromBody] UserDTO request)
        {
            // Input Validation: Ensure username and password are not empty
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            // Fetch the user from the database
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            // Verify the provided password against the stored hash
            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Generate JWT token for the authenticated user
                string token = CreateToken(user);
                return Ok(new { message = "Login successful.", token });
            }

            // If user is not found or password is invalid, return 401
            return Unauthorized(new { message = "Incorrect username or password." });
        }

        /// <summary>
        /// Creates a JWT token for an authenticated user.
        /// </summary>
        /// <param name="user">The authenticated user entity</param>
        /// <returns>JWT token string</returns>
        private string CreateToken(User user)
        {
            // Build a list of claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Symmetric key from appsettings
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"])
            );

            // Sign the token using the HmacSha512Signature algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Construct the JWT token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),    // Token expires in 1 day
                signingCredentials: creds
            );

            // Return the serialized token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
