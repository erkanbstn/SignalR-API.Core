using Microsoft.EntityFrameworkCore;

namespace Covid19Chart.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Covid> Covids { get; set; }
    }
}
