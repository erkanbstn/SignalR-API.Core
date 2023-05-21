using Microsoft.EntityFrameworkCore;

namespace SignalRUI.Core.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
