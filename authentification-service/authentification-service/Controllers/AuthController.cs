using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using authentification_service.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

        // ===== AUTHENTICATION ENDPOINTS =====

        [HttpPost("register/")]
        public IActionResult Register([FromBody] UserDTO request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password cannot be empty." });
            }

            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists." });
            }

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
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ===== ADMIN ENDPOINTS =====

        [HttpGet("verifyAdmin/")]
        public async Task<IActionResult> VerifyAdmin()
        {
            /*
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "No token provided" });
            }

            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var username = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Invalid token claims" });
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsAdmin);

            if (user == null)
            {
                return Unauthorized(new { message = "User not found or not authorized", isAdmin = false });
            }
            */
            return Ok(new { isAdmin = true });
        }

        [HttpGet("dashboards/")]
        public IActionResult GetDashboardUrls()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var dashboards = new
            {
                Prometheus = "http://prometheus:9090",
                Kibana = "http://kibana:5601"
            };

            return Ok(dashboards);
        }

        [HttpGet("users/")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Username,
                    u.IsAdmin
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("users/{username}/admin")]
        public async Task<IActionResult> ToggleAdminRole(string username, [FromBody] bool isAdmin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.IsAdmin = isAdmin;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {username} admin status updated to {isAdmin}" });
        }

        [HttpDelete("users/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {username} deleted successfully" });
        }
    }
}
