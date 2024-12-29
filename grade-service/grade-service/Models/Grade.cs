namespace grade_service.Models
{
    /// <summary>
    /// Represents an individual grade record in the system.
    /// </summary>
    public class Grade
    {
        /// <summary>
        /// The primary key for this grade record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The username to whom this grade belongs.
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
        /// The numeric score (e.g., 5.5 out of 7).
        /// </summary>
        public required double Mark { get; set; }

        /// <summary>
        /// The weight or importance of this grade in the overall calculation (e.g., 0.3 for 30%).
        /// </summary>
        public required double Weight { get; set; }
    }
}