using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using authentification_service.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("dashboards")]
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

        [HttpGet("users")]
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
