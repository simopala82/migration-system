using System.ComponentModel.DataAnnotations;
using Migration.DataAccess.Model;

namespace Migration.DataAccess.Services;

public interface IUserNewRepository
{
    Task<UserNew> Get(Guid newUserId);
    Task CreateNewUser(UserNew newUser);
    Task DeleteNewUser(Guid newUserId);
}

public class UserNewRepository : IUserNewRepository
{
    private readonly UserNewDbContext _newContext;

    public UserNewRepository(UserNewDbContext newContext)
    {
        _newContext = newContext;
    }

    public async Task<UserNew> Get(Guid newUserId)
    {
        var user = await _newContext.UsersNew.FindAsync(newUserId);
        return user ?? throw new ValidationException($"User {newUserId} does not exist");
    }

    public async Task CreateNewUser(UserNew newUser)
    {
        _newContext.UsersNew.Add(newUser);
        await _newContext.SaveChangesAsync();
    }

    public async Task DeleteNewUser(Guid newUserId)
    {
        var user = await _newContext.UsersNew.FindAsync(newUserId);
        if (user != null)
        {
            _newContext.UsersNew.Remove(user);
            await _newContext.SaveChangesAsync();
        }
    }
}
