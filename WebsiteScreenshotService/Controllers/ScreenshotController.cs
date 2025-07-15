using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using WebsiteScreenshotService.Controllers.Examples.Indentity;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.Services;

namespace WebsiteScreenshotService.Controllers;

[Authorize]
[ApiController]
[Route("[action]")]
public class ScreenshotController(ISubscriptionManager subscriptionManager, IScreenshotService screenshotService) : ControllerBase
{
    private readonly ISubscriptionManager _subscriptionManager = subscriptionManager;
    private readonly IScreenshotService _screenshotService = screenshotService;

    /// <summary>
    /// Captures a screenshot based on the specified options and returns the image file.
    /// </summary>
    /// <param name="screenshotOptions">The options for the screenshot such as the URL, dimensions, etc.</param>
    /// <returns>
    /// A screenshot image file if successful, or a BadRequest with an appropriate error message if not.
    /// </returns>
    /// <response code="200">Returns the screenshot image file.</response>
    /// <response code="400">User does not exist or has exceeded their available screenshots limit or Input data is invalid.</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost]
    [ProducesResponseType<FileStream>(StatusCodes.Status200OK, "image/png", "image/jpeg")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/json")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(MakeScreenshotResponseExample))]
    public async Task<IActionResult> MakeScreenshot(ScreenshotOptionsModel screenshotOptions)
    {
        if (!await _subscriptionManager.CanMakeScreenshotAsync())
            return BadRequest(new ErrorResponse("You cannot make screenshot any more because you ran out of available screenshots"));

        var subscriptionPlan = await _subscriptionManager.ScreenshotWasMadeAsync();

        if (subscriptionPlan is null)
            return BadRequest(new ErrorResponse("User does not exist"));

        var screenshotResult = await _screenshotService.MakeScreenshotAsync(screenshotOptions);

        if (!screenshotResult.IsSuccess)
        {
            await _subscriptionManager.IncrementScreenshotCountAsync();
            return BadRequest(new ErrorResponse(screenshotResult.ErrorMessage!));
        }

        return Ok(new { screenshotId = screenshotResult.Value });
    }
}
