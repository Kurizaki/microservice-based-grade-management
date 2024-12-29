using calculation_service.Models;
using System.Collections.Generic;
using System.Linq;

namespace calculation_service
{
    /// <summary>
    /// The Calculator class provides methods to compute 
    /// category-level averages and a final grade 
    /// for a given list of Grade objects.
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// Calculates average scores for each category, 
        /// taking into account the weight of each Grade.
        /// </summary>
        /// <param name="grades">
        /// A list of Grade objects, 
        /// each containing a Category, Score, and optional Weight.
        /// </param>
        /// <returns>
        /// A dictionary where the key is the category name (string) 
        /// and the value is the weighted average score for that category (double).
        /// </returns>
        public Dictionary<string, double> CalculateCategoryGrades(List<Grade> grades)
        {
            // Prepare a dictionary to store the average grade per category.
            var categoryGrades = new Dictionary<string, double>();

            // Filter out any invalid scores or weights:
            // Score must be between 1 and 7 inclusive, Weight must be > 0
            // Then group by Category.
            var groupedGrades = grades
                .Where(g => g.Score >= 1 && g.Score <= 7 && g.Weight > 0)
                .GroupBy(g => g.Category);

            // Iterate through each grouped category to calculate averages.
            foreach (var group in groupedGrades)
            {
                double weightedSum = 0;
                double totalWeight = 0;

                // Accumulate the weighted sum and total weight.
                foreach (var grade in group)
                {
                    // If Weight is null, default to 1.
                    double weight = grade.Weight ?? 1;
                    weightedSum += grade.Score * weight;
                    totalWeight += weight;
                }

                // Compute the average for this category (round to 2 decimal places).
                double categoryAverage = totalWeight == 0
                    ? 0
                    : Math.Round(weightedSum / totalWeight, 2);

                categoryGrades[group.Key] = categoryAverage;
            }

            return categoryGrades;
        }

        /// <summary>
        /// Calculates a final grade by averaging all category averages.
        /// </summary>
        /// <param name="categoryGrades">
        /// A dictionary of categories and their corresponding average scores.
        /// </param>
        /// <returns>
        /// The overall average score across all categories, 
        /// rounded to 2 decimal places.
        /// Returns 0 if there are no categories.
        /// </returns>
        public double CalculateFinalGrade(Dictionary<string, double> categoryGrades)
        {
            // If there are no categories, return 0.
            if (categoryGrades == null || categoryGrades.Count == 0)
                return 0;

            // Sum all category averages and divide by the number of categories.
            double sumOfGrades = categoryGrades.Values.Sum();
            return Math.Round(sumOfGrades / categoryGrades.Count, 2);
        }
    }
}
