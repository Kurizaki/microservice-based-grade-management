using Microsoft.AspNetCore.Mvc;
using calculation_service.Models;
using System.Collections.Generic;

namespace calculation_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradeController : ControllerBase
    {
        private readonly Calculator _calculator;

        public GradeController()
        {
            _calculator = new Calculator();
        }

        // POST: api/grade/calculate
        [HttpPost("calculate/")]
        public IActionResult CalculateGrade([FromBody] List<Grade> grades)
        {
            if (grades == null || grades.Count == 0)
            {
                return BadRequest("No grades provided.");
            }

            var categoryGrades = _calculator.CalculateCategoryGrades(grades);
            double finalGrade = _calculator.CalculateFinalGrade(categoryGrades);

            return Ok(new
            {
                CategoryGrades = categoryGrades,
                FinalGrade = finalGrade
            });
        }
    }
}
