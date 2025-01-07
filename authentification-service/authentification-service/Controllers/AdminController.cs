using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using authentification_service.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace authentification_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AUTHDB _context;

        public AdminController(AUTHDB context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("verify-admin/")]
        public async Task<IActionResult> VerifyAdmin()
        {
            try
            {
                Console.WriteLine("VerifyAdmin endpoint called");
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Authorization header: {authHeader}");

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    Console.WriteLine("No token provided or invalid token format");
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

                // Check if user exists and is admin in database
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsAdmin);

                if (user == null)
                {
                    return Unauthorized(new { message = "User not found or not authorized", isAdmin = false });
                }

                return Ok(new { isAdmin = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "Error verifying admin status", error = ex.Message });
            }
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
