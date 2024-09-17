using grade_service.Data;
using grade_service.Models;
using grade_service.ModelsDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace grade_service.Cntrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly DataContext _context;
        public GradeController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("AddGrade/")]
        public async Task<ActionResult<Grade>> CreateChallenge([FromBody] GradeDTO request)
        {
            if (request.Weight == null)
            {
                request.Weight = 1;
            }
            if (request.Mark > 7)
            {
                return BadRequest(new { message = "Invalid Mark, you can only iput numbers from 1 to 7" });
            }
            var grade = new Grade
            {
                Username = request.Username,
                Category = request.Category,
                Title = request.Title,
                Mark = request.Mark,
                Weight = request.Weight,
            };
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return Ok(grade);
        }
        [HttpGet("GetGradeFromUser/")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGradesForCurrentUser()
        {
            // Get the user ID from the JWT token
            var usernameClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usernameClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Parse the user ID from the claim (assuming it's stored as a string)
            string username = usernameClaim.Value;

            // Query the grades associated with the user ID
            var grades = await _context.Grades
                                       .Where(g => g.Username == username)
                                       .ToListAsync();

            if (!grades.Any())
            {
                return NotFound("No grades found for the current user.");
            }

            return Ok(grades);
        }
    }
}
