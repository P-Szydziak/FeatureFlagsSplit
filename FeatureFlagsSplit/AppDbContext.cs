using Microsoft.EntityFrameworkCore;

namespace FeatureFlagsSplit
{
    public class AppDbContext : DbContext
    {
        public DbSet<Meal> Meal { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}