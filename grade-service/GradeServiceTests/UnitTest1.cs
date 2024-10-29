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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace grade_service.Tests
{
    [TestClass]
    public class GradeControllerTests
    {
        private Mock<DataContext>? _mockContext;
        private GradeController? _controller;

        private DataContext? _context;


        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Use unique database instance for each test
                .Options;

            _context = new DataContext(options);
            _controller = new GradeController(_context);
        }




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



        [TestMethod]
        public async Task GetGradesFromUser_WithValidUser_ReturnsOkResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "testUser")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

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



        [TestMethod]
        public async Task GetGradesFromUser_WithInvalidUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());

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

        [TestMethod]
        public async Task DeleteGrade_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var grade = new Grade { Id = 1, Username = "testUser", Category = "testCategory", Title = "testTitle", Mark = 5, Weight = 2 };
            await _context.Grades.AddAsync(grade); // Add grade to in-memory DB
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteGrade(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }


        [TestMethod]
        public async Task DeleteGrade_WithInvalidId_ReturnsNotFoundResult()
        {
            // Act
            var result = await _controller.DeleteGrade(999); // Use an ID that doesn't exist

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual("Grade not found.", notFoundResult.Value);
        }


        [TestMethod]
        public async Task UpdateGrade_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var grade = new Grade { Id = 1, Username = "testUser", Category = "testCategory", Title = "testTitle", Mark = 5, Weight = 2 };
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
            var result = await _controller.UpdateGrade(999, request); // ID that does not exist

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result.Result;
            Assert.AreEqual("Grade not found.", notFoundResult.Value);
        }


    }
}