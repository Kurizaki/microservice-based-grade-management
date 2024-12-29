namespace calculation_service.Models
{
    /// <summary>
    /// Represents a single grade within a specific category,
    /// potentially including a weight factor for grade calculations.
    /// </summary>
    public class Grade
    {
        /// <summary>
        /// The name or type of category to which this grade belongs 
        /// (e.g., "Homework", "Exam", "Project").
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The numerical score for this grade (e.g., 85.5).
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// An optional weight (e.g., 0.3 for 30% weighting) 
        /// that might influence final grade calculations. 
        /// If null, a default or uniform weighting may be applied.
        /// </summary>
        public double? Weight { get; set; }
    }
}