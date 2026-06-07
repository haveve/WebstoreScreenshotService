using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using WebsiteScreenshotService.Controllers.Examples.Indentity;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.User)]
[ApiController]
[Route("screenshots/[action]")]
public class ScreenshotController(
    IScreenshotService screenshotService,
    IScreenshotManager screenshotManager,
    IScreenshotStorageManager screenshotStorageManager,
    IUserContextAccessor userContextAccessor,
    IScreenshotCalculator screenshotCalculator) : ControllerBase
{
    private readonly IScreenshotService _screenshotService = screenshotService;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;
    private readonly IScreenshotStorageManager _screenshotStorageManager = screenshotStorageManager;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IScreenshotCalculator _screenshotCalculator = screenshotCalculator;

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
    [Authorize(Policy = Policies.User.MakeScreenshots)]
    [ActionName("makeScreenshot")]
    [ProducesResponseType<FileStream>(StatusCodes.Status200OK, "image/png", "image/jpeg")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/json")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(MakeScreenshotResponseExample))]
    public async Task<IActionResult> MakeScreenshot([FromBody] InputScreenshotModel screenshotOptions)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var model = screenshotOptions.ToScreenshotOptions(userContext);

        if (!model.IsSuccess)
            return BadRequest(new ErrorResponse(model.ErrorMessage!));

        var screenshotResult = await _screenshotService.MakeScreenshotAsync(model.Value!);

        if (!screenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(screenshotResult.ErrorMessage!));

        var useInfo = userContext.UserInfo;
        var screenshot = screenshotResult.Value!;
        var responseModel = new ScreenshotModel(screenshot, _screenshotStorageManager.GetScreenshotUrl(screenshot, useInfo.Id));
        return Ok(responseModel);
    }

    [HttpPost]
    [Authorize(Policy = Policies.User.MakeScreenshots)]
    [ActionName("calculateScreenshotCost")]
    public IActionResult CalculateScreenshotCost([FromBody] InputScreenshotModel screenshotOptions)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var model = screenshotOptions.ToScreenshotOptions(userContext);

        if (!model.IsSuccess)
            return BadRequest(new ErrorResponse(model.ErrorMessage!));

        var pointsCost = _screenshotCalculator.CalculatePoints(model.Value!);
        var calculationModel = new ScreenshotCalculationModel(pointsCost);

        return Ok(calculationModel);
    }

    [HttpGet]
    [Authorize(Policy = Policies.User.FetchScreenshots)]
    [ActionName("getScreenshots")]
    public async Task<IActionResult> GetScreenshots([FromQuery] Paging paging)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var screenshotPaging = new ScreenshotPaging(paging.Page, paging.PageSize, paging.Query, paging.SearchScope, paging.CategoryIds);
        var screenshotResult = await _screenshotManager.GetScreenshots(screenshotPaging);

        if (!screenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(screenshotResult.ErrorMessage!));

        var result = screenshotResult.Value!;

        var useInfo = userContext.UserInfo;
        var paginationResult = new PaginationResult<ScreenshotModel>
        (
            TotalCount: result.TotalCount,
            Items: result.Items
                .Select(screenshot => new ScreenshotModel(screenshot, _screenshotStorageManager.GetScreenshotUrl(screenshot, useInfo.Id)))
                .ToArray()
        );

        return Ok(paginationResult);
    }

    [HttpGet]
    [Authorize(Policy = Policies.User.FetchScreenshots)]
    [ActionName("getScreenshot")]
    public async Task<IActionResult> GetScreenshot([FromQuery] string id)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var storedScreenshotResult = await _screenshotManager.GetScreenshot(id);

        if (!storedScreenshotResult.IsSuccess)
            return BadRequest(new ErrorResponse(storedScreenshotResult.ErrorMessage!));

        var useInfo = userContext.UserInfo;
        var storedScreenshot = storedScreenshotResult.Value!;
        var screenshot = new ScreenshotModel(storedScreenshot, _screenshotStorageManager.GetScreenshotUrl(storedScreenshot, useInfo.Id));
        return Ok(screenshot);
    }
}