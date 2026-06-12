using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteScreenshotService.Controllers;

[Authorize(Roles = UserRoles.User)]
[Route("admin/[action]")]
[ApiController]
public class AdminController: ControllerBase
{
}
