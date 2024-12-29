namespace grade_service.ModelsDTO
{
    /// <summary>
    /// A Data Transfer Object for carrying grade information
    /// without exposing the database entity directly.
    /// </summary>
    public class GradeDTO
    {
        /// <summary>
        /// The username that this grade belongs to.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// The category of the grade (e.g., "Homework", "Exam").
        /// </summary>
        public required string Category { get; set; }

        /// <summary>
        /// A short title or description of the grade (e.g., "Midterm Exam").
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// The numeric mark for this grade (e.g., 5.5 out of 7).
        /// </summary>
        public required double Mark { get; set; }

        /// <summary>
        /// The weight or relative importance of this grade
        /// in the overall calculation (e.g., 0.3 for 30%).
        /// </summary>
        public required double Weight { get; set; }
    }
}