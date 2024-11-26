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
        private readonly AUTHDB _context;
        private readonly IConfiguration _configuration;

        public AuthController(AUTHDB context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("register/")]
        public IActionResult Register([FromBody] UserDTO request)
        {
            // Input-Validierung
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            // Überprüfung auf existierenden Benutzer
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists." });
            }

            // Passwort-Hashing
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving user to the database.", error = ex.Message });
            }

            return Ok(new { message = "Registration successful.", username = newUser.Username });
        }

        [HttpPost("login/")]
        public IActionResult Login([FromBody] UserDTO request)
        {
            // Input-Validierung
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                string token = CreateToken(user);
                return Ok(new { message = "Login successful.", token });
            }

            return Unauthorized(new { message = "Incorrect username or password." });
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
