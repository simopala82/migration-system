using Microsoft.EntityFrameworkCore;
using Migration.DataAccess.Model;

namespace Migration.DataAccess;

public class UserNewDbContext : DbContext
{
    public DbSet<UserNew> UsersNew { get; set; }

    public UserNewDbContext(DbContextOptions<UserNewDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNew>().HasKey(u => u.UserId);
    }
}
