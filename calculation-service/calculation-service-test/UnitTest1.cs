using calculation_service;
using calculation_service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace calculation_service_test
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Tests grade calculation when all entries have equal weights.
        /// </summary>
        [TestMethod]
        public void CalculateGrade_SameWeights()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Category = "Math", Score = 6, Weight = 1 },
                new Grade { Category = "Science", Score = 4.8, Weight = 1 }
            };

            // Act
            var categoryGrades = calculator.CalculateCategoryGrades(grades);
            double finalGrade = calculator.CalculateFinalGrade(categoryGrades);

            // Assert
            Assert.AreEqual(6, categoryGrades["Math"]);
            Assert.AreEqual(4.8, categoryGrades["Science"]);
            Assert.AreEqual(5.4, finalGrade);
        }

        /// <summary>
        /// Tests grade calculation when entries have different weights.
        /// </summary>
        [TestMethod]
        public void CalculateGrade_DifferentWeights()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Category = "Biology", Score = 5.25, Weight = 0.5 },
                new Grade { Category = "Math", Score = 6, Weight = 0.5 },
                new Grade { Category = "Science", Score = 4.8, Weight = 1 }
            };

            // Act
            var categoryGrades = calculator.CalculateCategoryGrades(grades);
            double finalGrade = calculator.CalculateFinalGrade(categoryGrades);

            // Assert
            Assert.AreEqual(5.25, categoryGrades["Biology"]);
            Assert.AreEqual(6, categoryGrades["Math"]);
            Assert.AreEqual(4.8, categoryGrades["Science"]);
            // Allow a small tolerance for floating-point comparison
            Assert.AreEqual(5.35, finalGrade, 0.01);
        }

        /// <summary>
        /// Tests grade calculation when the list of grades is empty.
        /// </summary>
        [TestMethod]
        public void CalculateGrade_EmptyList()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>();

            // Act
            var categoryGrades = calculator.CalculateCategoryGrades(grades);
            double finalGrade = calculator.CalculateFinalGrade(categoryGrades);

            // Assert
            Assert.AreEqual(0, categoryGrades.Count);
            Assert.AreEqual(0, finalGrade);
        }

        /// <summary>
        /// Tests grade calculation when the list contains negative scores or negative weights.
        /// </summary>
        [TestMethod]
        public void CalculateGrade_NegativeValues()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Category = "Math", Score = -5, Weight = 1 },
                new Grade { Category = "Science", Score = 4.8, Weight = -1 }
            };

            // Act
            var categoryGrades = calculator.CalculateCategoryGrades(grades);
            double finalGrade = calculator.CalculateFinalGrade(categoryGrades);

            // Assert
            Assert.AreEqual(0, categoryGrades.Count);
            Assert.AreEqual(0, finalGrade);
        }

        /// <summary>
        /// Tests grade calculation when there are multiple entries for the same category.
        /// </summary>
        [TestMethod]
        public void CalculateGrade_MultipleEntries_SameCategory()
        {
            // Arrange
            var calculator = new Calculator();
            var grades = new List<Grade>
            {
                new Grade { Category = "Math", Score = 6, Weight = 1 },
                new Grade { Category = "Math", Score = 4.5, Weight = 0.5 },
                new Grade { Category = "Science", Score = 5, Weight = 1 },
                new Grade { Category = "Science", Score = 4, Weight = 2 }
            };

            // Act
            var categoryGrades = calculator.CalculateCategoryGrades(grades);
            double finalGrade = calculator.CalculateFinalGrade(categoryGrades);

            // Assert
            // A small tolerance is used due to floating-point arithmetic.
            Assert.AreEqual(5.5, categoryGrades["Math"], 0.01);
            Assert.AreEqual(4.3, categoryGrades["Science"], 0.05);
            Assert.AreEqual(4.9, finalGrade, 0.05);
        }
    }
}
