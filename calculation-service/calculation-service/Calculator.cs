using calculation_service.Models;
using System.Collections.Generic;

namespace calculation_service
{
    public class Calculator
    {
        public double CalculateGrade(List<Grade> grades)
        {
            if (grades == null || grades.Count == 0)
                return 0;

            double weightedSum = 0;
            double totalWeight = 0;

            foreach (var grade in grades)
            {
                if (grade.Score < 1 || grade.Score > 7)
                {
                    continue;
                }

                double weight = grade.Weight ?? 1;
                if (weight <= 0) weight = 1;

                weightedSum += grade.Score * weight;
                totalWeight += weight;
            }

            return totalWeight == 0 ? 0 : weightedSum / totalWeight;
        }

    }
}
