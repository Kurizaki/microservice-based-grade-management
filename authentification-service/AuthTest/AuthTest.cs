using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Configuration;
using authentification_service.Models;
using authentification_service.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest
{
    [TestClass]
    public class AuthTest
    {
        private readonly AuthController _controller;
        private readonly IConfiguration _configuration;

        public AuthTest()
        {
            // Set up in-memory database options
            var options = new DbContextOptionsBuilder<AUTHDB>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create an instance of AUTHDB with in-memory database
            var context = new AUTHDB(options);

            // Set up configuration mock
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c.GetSection("AppSettings:Token").Value).Returns("sadfghjkhgfdsewrtasdfgdsafgdasdfgshddsafghjdsafghjfdsafghjkfdsafghjk");
            _configuration = mockConfiguration.Object;

            // Initialize the controller with the in-memory context and configuration
            _controller = new AuthController(context, _configuration);
        }

        [TestMethod]
        public void Login_ReturnsOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";
            var user = new User { Username = username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) };

            // Add user to the in-memory database
            _controller.Register(new UserDTO { Username = username, Password = password });

            // Act
            var result = _controller.Login(new UserDTO { Username = username, Password = password });

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Login_ReturnsUnauthorizedResult_WhenCredentialsAreInvalid()
        {
            // Act
            var result = _controller.Login(new UserDTO { Username = "invaliduser", Password = "wrongpassword" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }
    }
}
