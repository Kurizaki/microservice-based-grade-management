using Microsoft.EntityFrameworkCore;

namespace authentification_service.Models
{
    public class AUTHDB : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AUTHDB(DbContextOptions<AUTHDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(s => s.Id);
        }
    }
}
