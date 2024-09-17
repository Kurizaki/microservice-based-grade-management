using grade_service.Models;
using Microsoft.EntityFrameworkCore;
namespace grade_service.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        
        public DbSet<Grade> Grades { get; set; }
    }
}
