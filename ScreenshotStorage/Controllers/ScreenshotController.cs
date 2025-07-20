using Microsoft.AspNetCore.Mvc;
using ScreenshotStorage;
using ScreenshotStorage.Services;

[ApiController]
[Route("screenshot")]
public class ScreenshotController(IScreenshotService screenshotService) : ControllerBase
{
    private readonly IScreenshotService _screenshotService = screenshotService;

    [HttpPost("{userId}/{screenshotId}")]
    public async Task<IActionResult> Save(string userId, string screenshotId, [FromBody] ScreenshotData data)
    {
        await _screenshotService.SaveAsync(userId, screenshotId, data);
        return Ok();
    }

    [HttpDelete("{userId}/{screenshotId}")]
    public async Task<IActionResult> Delete(string userId, string screenshotId)
    {
        var deleted = await _screenshotService.DeleteAsync(userId, screenshotId);
        return deleted ? Ok() : NotFound();
    }
}
