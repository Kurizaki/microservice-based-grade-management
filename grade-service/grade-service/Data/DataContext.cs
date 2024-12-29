using grade_service.Models;
using Microsoft.EntityFrameworkCore;

namespace grade_service.Data
{
    /// <summary>
    /// Represents the EF Core database context for the grade service.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Initializes the DbContext with the specified options (e.g., connection string).
        /// </summary>
        /// <param name="options">The options to configure the DbContext.</param>
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// A DbSet representing the "Grades" table in the database.
        /// </summary>
        public DbSet<Grade> Grades { get; set; }
    }
}