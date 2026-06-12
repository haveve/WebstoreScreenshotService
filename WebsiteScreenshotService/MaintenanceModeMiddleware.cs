using WebsiteScreenshotService.Services.Admin;

namespace WebsiteScreenshotService;

public class MaintenanceModeMiddleware(IAdminService adminService) : IMiddleware
{
    private readonly IAdminService _adminService = adminService;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var maintenance = await _adminService.GetMaintenanceModeAsync();

        if (maintenance.Value?.Enabled == true)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Service is under maintenance."
            });

            return;
        }

        await next(context);
    }
}