using grade_service.Data;
using grade_service.Models;
using grade_service.ModelsDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    }
}
