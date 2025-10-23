using Microsoft.EntityFrameworkCore;
using Migration.DataAccess.Model;
using System.ComponentModel.DataAnnotations;

namespace Migration.DataAccess.Services;

public interface IUserOldRepository
{
    Task<bool> Exists(int legacyUserId);
    Task<UserOld> Get(int legacyUserId);
    Task MarkAsMigrated(int legacyUserId);
    Task<int> GetTotalUserCount();
}

public class UserOldRepository: IUserOldRepository
{
    private readonly UserOldDbContext _oldContext;

    public UserOldRepository(UserOldDbContext oldContext)
    {
        _oldContext = oldContext;
    }

    public async Task<bool> Exists(int legacyUserId)
    {
        return await _oldContext.UsersOld.AnyAsync(x => x.LegacyUserId == legacyUserId && !x.IsMigrated);
    }

    public async Task<UserOld> Get(int legacyUserId)
    {
        var user = await _oldContext.UsersOld.FindAsync(legacyUserId);
        if (user == null || user.IsMigrated)
            throw new ValidationException($"User {legacyUserId} does not exist or has already been migrated");

        return user;
    }

    public async Task MarkAsMigrated(int legacyUserId)
    {
        var user = await _oldContext.UsersOld.FindAsync(legacyUserId);
        if (user == null || user.IsMigrated)
            throw new ValidationException($"User {legacyUserId} does not exist or has already been migrated");
        
        user.IsMigrated = true;
        await _oldContext.SaveChangesAsync();
    }

    public async Task<int> GetTotalUserCount()
    {
        return await _oldContext.UsersOld.CountAsync();
    }
}
