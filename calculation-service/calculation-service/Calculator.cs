using calculation_service.Models;
using System.Collections.Generic;
using System.Linq;

namespace calculation_service
{
    public class Calculator
    {
        public Dictionary<string, double> CalculateCategoryGrades(List<Grade> grades)
        {
            var categoryGrades = new Dictionary<string, double>();

            var groupedGrades = grades
                .Where(g => g.Score >= 1 && g.Score <= 7 && g.Weight > 0)
                .GroupBy(g => g.Category);

            foreach (var group in groupedGrades)
            {
                double weightedSum = 0;
                double totalWeight = 0;

                foreach (var grade in group)
                {
                    double weight = grade.Weight ?? 1;
                    weightedSum += grade.Score * weight;
                    totalWeight += weight;
                }

                double categoryAverage = totalWeight == 0 ? 0 : Math.Round(weightedSum / totalWeight, 2);
                categoryGrades[group.Key] = categoryAverage;
            }

            return categoryGrades;
        }

        public double CalculateFinalGrade(Dictionary<string, double> categoryGrades)
        {
            if (categoryGrades == null || categoryGrades.Count == 0)
                return 0;

            double sumOfGrades = categoryGrades.Values.Sum();
            return Math.Round(sumOfGrades / categoryGrades.Count, 2);
        }
    }
}
