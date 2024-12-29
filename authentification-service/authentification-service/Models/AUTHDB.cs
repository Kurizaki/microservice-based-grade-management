using Microsoft.EntityFrameworkCore;

namespace authentification_service.Models
{
    /// <summary>
    /// AUTHDB serves as the EF Core database context for user authentication.
    /// It declares the Users DbSet and configures the User entity.
    /// </summary>
    public class AUTHDB : DbContext
    {
        /// <summary>
        /// Represents the collection of User entities in the database.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Passes the DbContextOptions to the base DbContext constructor.
        /// </summary>
        /// <param name="options">Configuration options for the database context.</param>
        public AUTHDB(DbContextOptions<AUTHDB> options) : base(options)
        {
        }

        /// <summary>
        /// Further configures how the User entity is mapped to the database schema.
        /// Here, it defines the primary key for the User entity.
        /// </summary>
        /// <param name="modelBuilder">A ModelBuilder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(s => s.Id);  // Sets the Id property as the primary key
        }
    }
}