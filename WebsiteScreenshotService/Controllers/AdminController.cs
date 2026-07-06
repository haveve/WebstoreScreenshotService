using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Services.Admin;
using WebsiteScreenshotService.Services.Admin.Models;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.Admin)]
[Route("admin/[action]")]
[ApiController]
public class AdminController(IAdminService adminService) : ControllerBase
{
    private readonly IAdminService _adminService = adminService;

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics([FromQuery] AdminStatisticsQuery query)
    {
        var result = await _adminService.GetStatisticsAsync(query);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] AdminUserQuery query)
    {
        var result = await _adminService.GetUsersAsync(query);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserDetails(Guid userId)
    {
        var result = await _adminService.GetUserDetailsAsync(userId);

        if (!result.IsSuccess)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpPost("users/{userId:guid}/enable")]
    public async Task<IActionResult> EnableUser(Guid userId)
    {
        var result = await _adminService.EnableUserAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok();
    }

    [HttpPost("users/{userId:guid}/disable")]
    public async Task<IActionResult> DisableUser(Guid userId)
    {
        var result = await _adminService.DisableUserAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok();
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] AdminLogQuery query)
    {
        var result = await _adminService.GetLogsAsync(query);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        var result = await _adminService.GetHealthAsync();

        if (!result.IsSuccess)
            return StatusCode(503, result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpGet("maintenance")]
    public async Task<IActionResult> GetMaintenance()
    {
        var result = await _adminService.GetMaintenanceModeAsync();

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    [HttpPost("maintenance")]
    public async Task<IActionResult> SetMaintenance([FromBody] SetMaintenanceRequest request)
    {
        var result = await _adminService.SetMaintenanceModeAsync(request.Enabled);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok();
    }
}

public sealed record SetMaintenanceRequest(bool Enabled);
