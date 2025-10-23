using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Migration.Core.Services;

namespace Migration.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ISlotManager _slotManager;
    private readonly IMigrationManager _migrationManager;

    public AdminController(ISlotManager slotManager, IMigrationManager migrationManager)
    {
        _slotManager = slotManager;
        _migrationManager = migrationManager;
    }

    [HttpGet("slot/status")]
    [ProducesResponseType(200)]
    public IActionResult GetSlotStatus()
    {
        return Ok(new
        {
            _slotManager.MaxSlots,
            AvailableSlots = _slotManager.GetAvailableSlots(),
            SlotsInUse = _slotManager.MaxSlots - _slotManager.GetAvailableSlots()
        });
    }

    [HttpPost("migration/{legacyUserId}")]
    [ProducesResponseType(202)]
    public async Task<IActionResult> ForceMigration(int legacyUserId)
    {
        var adminUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown Admin";

        await _migrationManager.InitiateForcedMigration(legacyUserId, adminUser);

        return Accepted($"Forced migration for user {legacyUserId} started.");
    }

    [HttpGet("migration/status")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetMigrationStatus()
    {
        var totalUsers = await _migrationManager.GetTotalUserCount();
        var migratedCount = await _migrationManager.GetSuccessfulMigrationsCount();
        var failedCount = await _migrationManager.GetFailedMigrationsCount();

        return Ok(new
        {
            TotalUsers = totalUsers,
            MigratedPercentage = (double)migratedCount / totalUsers * 100,
            MigrationsInProgress = _slotManager.MaxSlots - _slotManager.GetAvailableSlots(),
            TotalSuccess = migratedCount,
            TotalErrors = failedCount
        });
    }
}
