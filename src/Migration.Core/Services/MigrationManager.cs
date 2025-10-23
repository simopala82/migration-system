using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Migration.DataAccess.Services;

namespace Migration.Core.Services;

public interface IMigrationManager
{
    Task<bool> UserOldCanBeMigrated(int legacyUserId);
    Task InitiateMigration(int legacyUserId);
    Task InitiateForcedMigration(int legacyUserId, string adminUser);
    Task<int> GetTotalUserCount();
    Task<int> GetSuccessfulMigrationsCount();
    Task<int> GetFailedMigrationsCount();
}

public class MigrationManager : IMigrationManager
{
    private readonly IMigrationStatusRepository _migrationStatusRepository;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<MigrationManager> _logger;
    private readonly IUserOldRepository _userOldRepository;

    public MigrationManager(IMigrationStatusRepository migrationStatusRepository, IMessageProducer messageProducer, ILogger<MigrationManager> logger, IUserOldRepository userOldRepository)
    {
        _migrationStatusRepository = migrationStatusRepository;
        _messageProducer = messageProducer;
        _logger = logger;
        _userOldRepository = userOldRepository;
    }

    public async Task InitiateMigration(int legacyUserId)
    {
        _logger.LogInformation($"Starting migration accepted by user {legacyUserId}");

        if (!await UserOldCanBeMigrated(legacyUserId))
            throw new ValidationException($"User {legacyUserId} does not exist, has already been migrated, or migration is in progress");

        await _migrationStatusRepository.Create(legacyUserId);
        await _messageProducer.SendMigrationRequest(legacyUserId);
    }

    public async Task<bool> UserOldCanBeMigrated(int legacyUserId)
    {
        if (!await _userOldRepository.Exists(legacyUserId))
            return false;

        if (await _migrationStatusRepository.Exists(legacyUserId))
            return false;

        return true;
    }

    public async Task InitiateForcedMigration(int legacyUserId, string adminUser)
    {
        _logger.LogInformation($"Starting forced migration for user {legacyUserId}");
        
        if (!await UserOldCanBeMigrated(legacyUserId))
            throw new ValidationException($"User {legacyUserId} does not exist, has already been migrated, or migration is in progress");

        await _migrationStatusRepository.CreateForced(legacyUserId, adminUser);
        await _messageProducer.SendForcedMigrationRequest(legacyUserId, adminUser);
    }
    
    public Task<int> GetTotalUserCount()
    {
        return _userOldRepository.GetTotalUserCount();
    }

    public Task<int> GetSuccessfulMigrationsCount()
    {
        return _migrationStatusRepository.GetSuccessfulMigrationsCount();
    }

    public Task<int> GetFailedMigrationsCount()
    {
        return _migrationStatusRepository.GetFailedMigrationsCount();
    }
}
