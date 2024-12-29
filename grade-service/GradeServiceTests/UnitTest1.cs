using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using grade_service.Controllers;
using grade_service.Data;
using grade_service.Models;
using grade_service.ModelsDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace grade_service.Tests
{
    [TestClass]
    public class GradeControllerTests
    {
        private DataContext? _context;
        private GradeController? _controller;

        /// <summary>
        /// Runs before each test to configure an in-memory DataContext
        /// and instantiate the GradeController with that context.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Use a unique in-memory database each time to avoid state carryover
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _controller = new GradeController(_context);
        }

        /// <summary>
        /// Verifies that a valid GradeDTO creates a Grade record
        /// and returns an OkObjectResult.
        /// </summary>
        [TestMethod]
        public async Task CreateGrade_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new GradeDTO
            {
                Username = "testUser",
                Category = "testCategory",
                Title = "testTitle",
                Mark = 5,
                Weight = 2
            };

            // Act
            var result = await _controller.CreateChallenge(request);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(Grade));
        }

        /// <summary>
        /// Tests that providing an invalid Mark (e.g., 8) returns a BadRequest.
        /// </summary>
        [TestMethod]
        public async Task CreateGrade_WithInvalidMark_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new GradeDTO
            {
                Username = "testUser",
                Category = "testCategory",
                Title = "testTitle",
                Mark = 8,
                Weight = 2
            };

            // Act
            var result = await _controller.CreateChallenge(request);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid Mark, you can only input numbers from 1 to 7", badRequestResult.Value);
        }

        /// <summary>
        /// Verifies that an authenticated user with valid claims
        /// can retrieve their own grades, returning OkObjectResult with a list of grades.
        /// </summary>
        [TestMethod]
        public async Task GetGradesFromUser_WithValidUser_ReturnsOkResult()
        {
            // Arrange
            // Create a mock user identity with a Name claim
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testUser")
            }));

            // Attach this user to the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Insert some grades into the in-memory DB
            var grades = new List<Grade>
            {
                new Grade { Id = 1, Username = "testUser", Category = "testCategory", Title = "testTitle", Mark = 5, Weight = 2 },
                new Grade { Id = 2, Username = "testUser", Category = "testCategory2", Title = "testTitle2", Mark = 4, Weight = 1 }
            };
            await _context.Grades.AddRangeAsync(grades);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetGradesFromUser();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<Grade>));
            var gradesResult = (List<Grade>)okResult.Value;
            Assert.AreEqual(2, gradesResult.Count);
        }

        /// <summary>
        /// Tests that a user without a valid Name claim
        /// is considered unauthorized.
        /// </summary>
        [TestMethod]
        public async Task GetGradesFromUser_WithInvalidUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            // Create a user with no claims
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Attach the user to controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.GetGradesFromUser();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = (UnauthorizedObjectResult)result.Result;
            Assert.AreEqual("User not authenticated.", unauthorizedResult.Value);
        }

        /// <summary>
        /// Verifies that deleting an existing Grade by valid ID
        /// returns a NoContentResult.
        /// </summary>
        [TestMethod]
        public async Task DeleteGrade_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var grade = new Grade
            {
                Id = 1,
                Username = "testUser",
                Category = "testCategory",
                Title = "testTitle",
                Mark = 5,
                Weight = 2
            };
            // Add grade to in-memory DB
            await _context.Grades.AddAsync(grade);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteGrade(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        /// <summary>
        /// Tests that attempting to delete a non-existent Grade
        /// returns a NotFoundObjectResult.
        /// </summary>
        [TestMethod]
        public async Task DeleteGrade_WithInvalidId_ReturnsNotFoundResult()
        {
            // Act
            var result = await _controller.DeleteGrade(999); // Non-existent ID

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual("Grade not found.", notFoundResult.Value);
        }

        /// <summary>
        /// Verifies that updating an existing Grade
        /// returns an OkObjectResult containing the updated Grade.
        /// </summary>
        [TestMethod]
        public async Task UpdateGrade_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var grade = new Grade
            {
                Id = 1,
                Username = "testUser",
                Category = "testCategory",
                Title = "testTitle",
                Mark = 5,
                Weight = 2
            };
            await _context.Grades.AddAsync(grade);
            await _context.SaveChangesAsync();

            var request = new GradeDTO
            {
                Username = "newTestUser",
                Category = "newTestCategory",
                Title = "newTestTitle",
                Mark = 6,
                Weight = 3
            };

            // Act
            var result = await _controller.UpdateGrade(1, request);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.IsInstanceOfType(okResult.Value, typeof(Grade));
            var updatedGrade = (Grade)okResult.Value;
            Assert.AreEqual("newTestUser", updatedGrade.Username);
            Assert.AreEqual("newTestCategory", updatedGrade.Category);
            Assert.AreEqual("newTestTitle", updatedGrade.Title);
            Assert.AreEqual(6, updatedGrade.Mark);
            Assert.AreEqual(3, updatedGrade.Weight);
        }

        /// <summary>
        /// Tests that updating a non-existent Grade
        /// returns a NotFoundObjectResult.
        /// </summary>
        [TestMethod]
        public async Task UpdateGrade_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new GradeDTO
            {
                Username = "newTestUser",
                Category = "newTestCategory",
                Title = "newTestTitle",
                Mark = 6,
                Weight = 3
            };

            // Act
            var result = await _controller.UpdateGrade(999, request);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result.Result;
            Assert.AreEqual("Grade not found.", notFoundResult.Value);
        }
    }
}
