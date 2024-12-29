using grade_service.Data;
using grade_service.Models;
using grade_service.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace grade_service.Controllers
{
    /// <summary>
    /// A controller handling CRUD operations for Grade entities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly DataContext _context;

        public GradeController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new Grade record in the database.
        /// POST: api/Grade/AddGrade
        /// </summary>
        /// <param name="request">DTO with grade info (Username, Category, Title, Mark, Weight).</param>
        /// <returns>
        /// The newly created Grade (HTTP 200) if successful,
        /// or an error message (HTTP 400 / 500) otherwise.
        /// </returns>
        [HttpPost("AddGrade/")]
        public async Task<ActionResult<Grade>> CreateChallenge([FromBody] GradeDTO request)
        {
            try
            {
                // Ensure default weight of 1 if none is provided
                if (request.Weight == null)
                {
                    request.Weight = 1;
                }

                // Validate Mark range
                if (request.Mark > 7 || request.Mark < 1)
                {
                    return BadRequest("Invalid Mark, you can only input numbers from 1 to 7.");
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
                // Log the error to console for diagnostic purposes
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves all grades for the authenticated user.
        /// GET: api/Grade/GetGradesFromUser
        /// Authorization: Bearer [JWT token]
        /// </summary>
        /// <returns>
        /// A list of Grade objects (HTTP 200) or an error/empty message otherwise.
        /// </returns>
        [Authorize]
        [HttpGet("GetGradesFromUser")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGradesFromUser()
        {
            // Extract the username from the JWT claims
            var username = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (username == null)
            {
                return Unauthorized("User not authenticated.");
            }

            // Fetch all grades belonging to this user
            var grades = await _context.Grades
                .Where(g => g.Username == username)
                .ToListAsync();

            if (grades == null || !grades.Any())
            {
                return NotFound("No grades found for the specified user.");
            }

            return Ok(grades);
        }

        /// <summary>
        /// Deletes a single Grade record by its ID.
        /// DELETE: api/Grade/DeleteGrade/{id}
        /// </summary>
        /// <param name="id">The ID of the Grade record.</param>
        /// <returns>
        /// NoContent (HTTP 204) if deleted successfully,
        /// or NotFound (HTTP 404) if the record does not exist.
        /// </returns>
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

        /// <summary>
        /// Updates a Grade record by its ID.
        /// PUT: api/Grade/UpdateGrade/{id}
        /// </summary>
        /// <param name="id">The ID of the Grade record.</param>
        /// <param name="request">DTO with updated grade info.</param>
        /// <returns>
        /// The updated Grade (HTTP 200) if successful,
        /// or NotFound (HTTP 404) if the record does not exist.
        /// </returns>
        [HttpPut("UpdateGrade/{id}")]
        public async Task<ActionResult<Grade>> UpdateGrade(int id, [FromBody] GradeDTO request)
        {
            var grade = await _context.Grades.FindAsync(id);

            if (grade == null)
            {
                return NotFound("Grade not found.");
            }

            // Update fields
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
