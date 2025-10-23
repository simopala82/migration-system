using Microsoft.EntityFrameworkCore;
using Migration.DataAccess.Model;

namespace Migration.DataAccess;

public class MigrationDbContext : DbContext
{
    public DbSet<MigrationStatus> MigrationStatus { get; set; }

    public MigrationDbContext(DbContextOptions<MigrationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
            
        // Primary key and index configuration
        modelBuilder.Entity<MigrationStatus>()
            .HasIndex(l => l.LegacyUserId)
            .IsUnique(); 

        modelBuilder.Entity<MigrationStatus>()
            .Property(l => l.Status)
            .HasMaxLength(50);
    }
}
