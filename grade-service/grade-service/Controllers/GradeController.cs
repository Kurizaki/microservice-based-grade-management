using grade_service.Data;
using grade_service.Models;
using grade_service.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace grade_service.Controllers
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
            try
            {
                if (request.Weight == null)
                {
                    request.Weight = 1;
                }

                if (request.Mark > 7 || request.Mark < 1)
                {
                    return BadRequest("Invalid Mark, you can only input numbers from 1 to 7");
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
            catch (Exception ex)
            {
                // Log the error
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet("GetGradesFromUser")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGradesFromUser()
        {
            // Get the username from JWT claims
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (username == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Query the grades for the authenticated user
            var grades = await _context.Grades
                .Where(g => g.Username == username)
                .ToListAsync();

            if (grades == null || !grades.Any())
            {
                return NotFound("No grades found for the specified user.");
            }

            return Ok(grades);
        }

        [HttpDelete("DeleteGrade/{id}")]
        public async Task<ActionResult> DeleteGrade(int id)
        {
            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return NotFound("Grade not found.");
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("UpdateGrade/{id}")]
        public async Task<ActionResult<Grade>> UpdateGrade(int id, [FromBody] GradeDTO request)
        {
            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return NotFound("Grade not found.");
            }

            grade.Username = request.Username;
            grade.Category = request.Category;
            grade.Title = request.Title;
            grade.Mark = request.Mark;
            grade.Weight = request.Weight;

            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();

            return Ok(grade);
        }
    }
}
