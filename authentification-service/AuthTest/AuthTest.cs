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

        /// <summary>
        /// Initializes the in-memory database, mock configuration, 
        /// and AuthController for testing.
        /// </summary>
        public AuthTest()
        {
            // 1. Set up in-memory database options
            var options = new DbContextOptionsBuilder<AUTHDB>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // 2. Create an in-memory AUTHDB context
            var context = new AUTHDB(options);

            // 3. Mock IConfiguration to provide a dummy JWT token
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(c => c.GetSection("AppSettings:Token").Value)
                .Returns("sadfghjkhgfdsewrtasdfgdsafgdasdfgshddsafghjdsafghjfdsafghjkfdsafghjk");
            _configuration = mockConfiguration.Object;

            // 4. Initialize the controller using the in-memory context and mock config
            _controller = new AuthController(context, _configuration);
        }

        /// <summary>
        /// Verifies that a valid login returns an OkObjectResult.
        /// </summary>
        [TestMethod]
        public void Login_ReturnsOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";

            // Register a new user (which stores it in the in-memory database)
            _controller.Register(new UserDTO { Username = username, Password = password });

            // Act
            var result = _controller.Login(new UserDTO { Username = username, Password = password });

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Verifies that an invalid login returns an UnauthorizedObjectResult.
        /// </summary>
        [TestMethod]
        public void Login_ReturnsUnauthorizedResult_WhenCredentialsAreInvalid()
        {
            // Act: Attempt login with non-existent user credentials
            var result = _controller.Login(new UserDTO { Username = "invaliduser", Password = "wrongpassword" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }
    }
}
