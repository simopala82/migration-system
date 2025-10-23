using Microsoft.Extensions.Logging;
using Migration.Core.Services;
using Migration.DataAccess.Services;
using System.ComponentModel.DataAnnotations;
using Migration.DataAccess.Model;

namespace Migration.Worker;

public class MigrationProcessor
{
    private readonly ISlotManager _slotManager;
    private readonly IDataTransformer _transformer;
    private readonly IUserOldRepository _oldUserRepository;
    private readonly IUserNewRepository _newUserRepository;
    private readonly IMigrationStatusRepository _statusRepository;
    private readonly ILogger<MigrationProcessor> _logger;

    public MigrationProcessor(ISlotManager slotManager, IDataTransformer transformer,
        IUserOldRepository oldUserRepository, IUserNewRepository newUserRepository,
        IMigrationStatusRepository statusRepository, ILogger<MigrationProcessor> logger)
    {
        _slotManager = slotManager;
        _transformer = transformer;
        _oldUserRepository = oldUserRepository;
        _newUserRepository = newUserRepository;
        _statusRepository = statusRepository;
        _logger = logger;
    }

    public async Task ProcessMigrationAsync(int legacyUserId, CancellationToken stoppingToken)
    {
        Guid? newUserId;
        UserNew newUser;

        try
        {
            var oldUser = await _oldUserRepository.Get(legacyUserId); 
            
            newUser = _transformer.Transform(oldUser);
            newUserId = newUser.UserId;
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, $"Validation failed for user {legacyUserId}. No slot wasted.");
            return; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Old db lookup failed for user {legacyUserId}. No slot wasted.");
            return;
        }

        var slotAcquired = false;

        try
        {
            slotAcquired = await _slotManager.AcquireSlotAsync(stoppingToken);
            if (!slotAcquired)
            {
                _logger.LogWarning($"Unable to acquire slot for {legacyUserId}. Returning.");
                return;
            }
            
            _logger.LogInformation($"Slot acquired for migration {legacyUserId}.");

            await _newUserRepository.CreateNewUser(newUser);

            await _statusRepository.UpdateStatusToSuccess(legacyUserId, newUserId.Value);
            
            await _oldUserRepository.MarkAsMigrated(legacyUserId);
            
            _logger.LogInformation($"Migration completed successfully for {legacyUserId}. New ID: {newUserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Migration failed for {legacyUserId}. Starting compensation.");
            
            await _statusRepository.UpdateStatusToFailed(legacyUserId, ex);

            _logger.LogWarning($"Executing rollback: deleting NEW user {newUserId.Value}");
            await _newUserRepository.DeleteNewUser(newUserId.Value);
        }
        finally
        {
            if (slotAcquired) 
                _slotManager.ReleaseSlot();
        }
    }
}
