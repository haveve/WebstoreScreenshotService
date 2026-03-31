using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using WebsiteScreenshotService.Controllers.Examples.Indentity;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Model.ScreenshotOptions;
using WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Controllers;

[Authorize]
[ApiController]
[Route("[action]")]
public class ScreenshotController(IScreenshotService screenshotService, IScreenshotManager screenshotManager, IScreenshotStorageManager screenshotStorageManager, IUserContextAccessor userContextAccessor) : ControllerBase
{
    private readonly IScreenshotService _screenshotService = screenshotService;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;
    private readonly IScreenshotStorageManager _screenshotStorageManager = screenshotStorageManager;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;

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
    public async Task<IActionResult> MakeScreenshot([FromBody] InputScreenshotOptionsModel screenshotOptions)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var model = screenshotOptions.ToScreenshotOptions(userContext);

        if(!model.IsSuccess)
            return BadRequest(new ErrorResponse(model.ErrorMessage!));

        var screenshotResult = await _screenshotService.MakeScreenshotAsync(model.Value!);

        if (!screenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(screenshotResult.ErrorMessage!));

        var screenshot = screenshotResult.Value!;
        var responseModel = new ScreenshotModel(screenshot, _screenshotStorageManager.GetScreenshotUrl(screenshot));
        return Ok(responseModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetScreenshots([FromQuery] Paging paging)
    {
        var screenshotResult = await _screenshotManager.GetScreenshots(paging);

        if (!screenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(screenshotResult.ErrorMessage!));

        var screenshotPaging = screenshotResult.Value!;

        var paginationResult = new PaginationResult<ScreenshotModel>
        (
            TotalCount: screenshotPaging.TotalCount,
            Items: screenshotPaging.Items
                .Select(screenshot => new ScreenshotModel(screenshot, _screenshotStorageManager.GetScreenshotUrl(screenshot)))
        );

        return Ok(paginationResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetScreenshot([FromQuery] string id)
    {
        var storedScreenshotResult = await _screenshotManager.GetScreenshot(id);

        if (!storedScreenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(storedScreenshotResult.ErrorMessage!));

        var storedScreenshot = storedScreenshotResult.Value!;

        var screenshot = new ScreenshotModel(storedScreenshot, _screenshotStorageManager.GetScreenshotUrl(storedScreenshot));

        return Ok(screenshot);
    }
}