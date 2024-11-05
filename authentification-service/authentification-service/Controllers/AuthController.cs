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
        public static User user = new();
        private readonly AUTHDB _context;
        private readonly IConfiguration _configuration;

        public AuthController(AUTHDB context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("register/")]
        public IActionResult Register([FromBody] UserDTO request)
        {

            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists. " });
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(newUser);

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUser(int id) 
        {
            
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            

            return NoContent();
        }

        [HttpPost("login/")]
        public IActionResult Login([FromBody] UserDTO request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                string token = CreateToken(user);
                return Ok(token);
            }
            return Unauthorized(new { message = "Falsches Passwort und oder flascher Benutzername." });
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
    

