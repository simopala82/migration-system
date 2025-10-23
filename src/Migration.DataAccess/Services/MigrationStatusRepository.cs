using Microsoft.EntityFrameworkCore;
using Migration.DataAccess.Model;

namespace Migration.DataAccess.Services;

public interface IMigrationStatusRepository
{
    Task<int> GetSuccessfulMigrationsCount();
    Task<int> GetFailedMigrationsCount();

    Task<bool> Exists(int legacyUserId);

    Task Create(int legacyUserId);
    Task CreateForced(int legacyUserId, string adminUser);

    Task UpdateStatusToSuccess(int legacyUserId, Guid newUserId);
    Task UpdateStatusToFailed(int legacyUserId, Exception ex);
}

public class MigrationStatusRepository : IMigrationStatusRepository
{
    private readonly MigrationDbContext _dbContext;

    public MigrationStatusRepository(MigrationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> GetSuccessfulMigrationsCount()
    {
        return _dbContext.MigrationStatus.CountAsync(l => l.Status == MigrationState.SUCCESS);
    }

    public Task<int> GetFailedMigrationsCount()
    {
        return _dbContext.MigrationStatus.CountAsync(l => l.Status == MigrationState.FAILED);
    }
    
    public async Task<bool> Exists(int legacyUserId)
    {
        return await _dbContext.MigrationStatus.AnyAsync(x => x.LegacyUserId == legacyUserId);
    }

    public async Task Create(int legacyUserId)
    {
        var logEntry = new MigrationStatus
        {
            LegacyUserId = legacyUserId,
            Status = MigrationState.PENDING,
            StartTime = DateTime.UtcNow
        };
        _dbContext.MigrationStatus.Add(logEntry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateForced(int legacyUserId, string adminUser)
    {
        var logEntry = new MigrationStatus
        {
            LegacyUserId = legacyUserId,
            Status = MigrationState.PENDING,
            StartTime = DateTime.UtcNow,
            AdminActionBy = adminUser
        };
        _dbContext.MigrationStatus.Add(logEntry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateStatusToSuccess(int legacyUserId, Guid newUserId)
    {
        var logEntry = await _dbContext.MigrationStatus.FirstAsync(l => l.LegacyUserId == legacyUserId);
        
        logEntry.Status = MigrationState.SUCCESS;
        logEntry.NewUserId = newUserId;
        logEntry.EndTime = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateStatusToFailed(int legacyUserId, Exception ex)
    {
        var logEntry = await _dbContext.MigrationStatus.FirstOrDefaultAsync(l => l.LegacyUserId == legacyUserId);

        if (logEntry != null)
        {
            logEntry.Status = MigrationState.FAILED;
            logEntry.EndTime = DateTime.UtcNow;

            logEntry.ErrorDetails = $"Error: {ex.Message} | Stack: {ex.StackTrace?[..Math.Min(ex.StackTrace.Length, 500)]}";
            await _dbContext.SaveChangesAsync();
        }
    }
}
