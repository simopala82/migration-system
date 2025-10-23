using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migration.DataAccess.Services;

namespace Migration.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
public class UserNewController : ControllerBase
{
    private readonly IUserNewRepository _userNewRepository;

    public UserNewController(IUserNewRepository userNewRepository)
    {
        _userNewRepository = userNewRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetUser()
    {
        var newUserId = GetUserIdFromClaims(); 

        var user = await _userNewRepository.Get(newUserId);
        return Ok(new
        {
            user.FullName
        });
    }

    private Guid GetUserIdFromClaims() 
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var newUserId))
            return newUserId;

        throw new UnauthorizedAccessException("Invalid or missing user ID in the authenticated request.");
    }
}
