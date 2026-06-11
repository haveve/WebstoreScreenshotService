using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteScreenshotService.Controllers;

[Authorize]
[ApiController]
[Route("subscription/[action]")]
public class SubscriptionController : ControllerBase
{
}
