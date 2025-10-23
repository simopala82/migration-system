using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Migration.Core.Services;
using Migration.DataAccess.Services;

namespace Migration.API.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
[ApiController]
public class UserOldController : ControllerBase
{
    private readonly ISlotManager _slotManager;
    private readonly IUserOldRepository _userOldRepository;
    private readonly IMigrationManager _migrationManager;

    public UserOldController(ISlotManager slotManager, IUserOldRepository userOldRepository, IMigrationManager migrationManager)
    {
        _slotManager = slotManager;
        _userOldRepository = userOldRepository;
        _migrationManager = migrationManager;
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetUser()
    {
        var legacyUserId = GetUserIdFromClaims(); 

        var user = await _userOldRepository.Get(legacyUserId);
        return Ok(new
        {
            user.FirstName,
            user.LastName,
            Migrated = user.IsMigrated,
            CanBeMigrated = await _migrationManager.UserOldCanBeMigrated(legacyUserId) && _slotManager.GetAvailableSlots() > 0
        });
    }

    [HttpPost("migration/accept")]
    [ProducesResponseType(202)]
    [ProducesResponseType(429)]
    public async Task<IActionResult> AcceptMigration()
    {
        var legacyUserId = GetUserIdFromClaims(); 

        if (_slotManager.GetAvailableSlots() <= 0)
            return StatusCode(429, "The system is temporarily saturated. Please try again later.");
        
        await _migrationManager.InitiateMigration(legacyUserId);

        return Accepted(new
        {
            message = "Your migration request has been accepted and will be processed shortly."
        });
    }

    private int GetUserIdFromClaims() 
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var legacyUserId))
            return legacyUserId;

        throw new UnauthorizedAccessException("Invalid or missing user ID in the authenticated request.");
    }
}
