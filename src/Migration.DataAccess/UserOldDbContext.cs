using Microsoft.EntityFrameworkCore;
using Migration.DataAccess.Model;

namespace Migration.DataAccess;

public class UserOldDbContext : DbContext
{
    public DbSet<UserOld> UsersOld { get; set; }

    public UserOldDbContext(DbContextOptions<UserOldDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserOld>().HasKey(u => u.LegacyUserId);
        modelBuilder.Entity<UserOld>().Property<bool>("IsMigrated").HasDefaultValue(false);

        AddTestData(modelBuilder);
    }

    private static ModelBuilder AddTestData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserOld>().HasData(
            new UserOld
            {
                LegacyUserId = 1,
                FirstName = "Mario",
                LastName = "Bianchi",
                LegacyEmailAddress = "mario.bianchi@email.com",
                Status = "ATTIVO",
                DocumentType = "FACTURE",
                IsMigrated = false
            },
            new UserOld
            {
                LegacyUserId = 2,
                FirstName = "Marco",
                LastName = "Verdi",
                LegacyEmailAddress = "marco.verdi@email.com",
                Status = "ATTIVO",
                DocumentType = "FACTURE",
                IsMigrated = false
            }
        );
        return modelBuilder;
    }
}
