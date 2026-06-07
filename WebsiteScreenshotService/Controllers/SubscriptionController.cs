using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.User)]
[ApiController]
[Route("subscription/[action]")]
public class SubscriptionController : ControllerBase
{
}
