using Microsoft.AspNetCore.Mvc;
using calculation_service.Models;
using System.Collections.Generic;

namespace calculation_service.Controllers
{
    /// <summary>
    /// A REST controller for performing various grade calculations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CalcController : ControllerBase
    {
        private readonly Calculator _calculator;

        /// <summary>
        /// Constructor initializes a new instance of the Calculator class.
        /// </summary>
        public CalcController()
        {
            _calculator = new Calculator();
        }

        /// <summary>
        /// Calculates category-based and final grades.
        /// POST: api/Calc/calculate
        /// </summary>
        /// <param name="grades">A list of Grade objects representing individual grades.</param>
        /// <returns>
        /// HTTP 400 (BadRequest) if no grades are provided,
        /// otherwise HTTP 200 (OK) with JSON containing CategoryGrades and FinalGrade.
        /// </returns>
        [HttpPost("calculate")]
        public IActionResult CalculateGrade([FromBody] List<Grade> grades)
        {
            // Validate that the request body is not null or empty
            if (grades == null || grades.Count == 0)
            {
                return BadRequest("No grades provided.");
            }

            // Calculate category-based grades (e.g., average per category)
            var categoryGrades = _calculator.CalculateCategoryGrades(grades);

            // Calculate the overall final grade from the category grades
            double finalGrade = _calculator.CalculateFinalGrade(categoryGrades);

            // Return both category-level grades and the final grade in a JSON response
            return Ok(new
            {
                CategoryGrades = categoryGrades,
                FinalGrade = finalGrade
            });
        }
    }
}