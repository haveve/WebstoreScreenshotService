using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using Swashbuckle.AspNetCore.Filters;
using WebsiteScreenshotService.Controllers.Examples.Indentity;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.Services;

namespace WebsiteScreenshotService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[action]")]
    public class ScreenshotContoller(IUserRepository UserRepository, IBrowserService BrowserService) : ControllerBase
    {
        private readonly IUserRepository _userRepository = UserRepository;
        private readonly IBrowserService _browserService = BrowserService;

        /// <summary>
        /// Captures a screenshot based on the specified options and returns the image file.
        /// </summary>
        /// <param name="screenshotOptions">The options for the screenshot such as the URL, dimensions, etc.</param>
        /// <returns>
        /// A screenshot image file if successful, or a BadRequest with an appropriate error message if not.
        /// </returns>
        /// <response code="200">Returns the screenshot image file.</response>
        /// <response code="400">User does not exist or has exceeded their available screenshots limit or Input data is invalid.</response>
        [HttpPost]
        [ProducesResponseType<FileStream>(StatusCodes.Status200OK, "image/png", "image/webp", "image/jpeg")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, "application/json")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(MakeScreenshotResponseExample))]
        public async Task<IActionResult> MakeScreenshot(ScreenshotOptionsModel screenshotOptions)
        {
            var userId = User.GetUserId()!.Value;
            var subscriptionPlan = await _userRepository.ScreenshotWasMade(userId);

            if (subscriptionPlan is null)
                return BadRequest("User does not exist");

            if (!subscriptionPlan.CanMakeScreenshot())
                return BadRequest("You cannot make screenshot any more because you ran out of avaialbe screenshots");

            var screenshotStream = await _browserService.MakeScreenshotAsync(screenshotOptions);
            var screenshotType = GetImageType(screenshotOptions.ScreenshotType);

            return File(screenshotStream, screenshotType);
        }

        private static string GetImageType(ScreenshotType screenshotType)
            => screenshotType switch
            {
                ScreenshotType.Png => "image/png",
                ScreenshotType.Jpeg => "image/jpeg",
                ScreenshotType.Webp => "image/webp",
                _ => throw new NotImplementedException("Invalid image type")
            };
    }

}
