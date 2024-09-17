using calculation_service;
using calculation_service.Models;

namespace calculation_service_test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CalculateGrade_SameWeights()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Name = "Math", Score = 6, Weight = 1 },
                new Grade { Name = "Science", Score = 4.8, Weight = 1 }
            };

            // Act
            var result = calculator.CalculateGrade(grades);

            // Assert
            Assert.AreEqual(5.4, result);
        }

        [TestMethod]
        public void CalculateGrade_DifferentWeights()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Name = "Biology", Score = 5.25, Weight = 0.5 },
                new Grade { Name = "Math", Score = 6, Weight = 0.5 },
                new Grade { Name = "Science", Score = 4.8, Weight = 1 }
            };

            // Act
            var result = calculator.CalculateGrade(grades);

            // Assert
            Assert.AreEqual(5.21, result, 1);
        }

        [TestMethod]
        public void CalculateGrade_EmptyList()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>();

            // Act
            var result = calculator.CalculateGrade(grades);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateGrade_NegativeValues()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Name = "Math", Score = -5, Weight = 1 },
                new Grade { Name = "Science", Score = 4.8, Weight = -1 }
            };

            // Act
            var result = calculator.CalculateGrade(grades);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}