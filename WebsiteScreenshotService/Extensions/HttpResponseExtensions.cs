using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Extensions;

public static class HttpResponseExtensions
{
    public static async Task UnauthorizedAccess(this HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        var errorMessage = new ErrorResponse("Access denied due to invalid credentials");
        await response.WriteAsJsonAsync(errorMessage);
    }

    public static async Task InternalServerError(this HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status500InternalServerError;
        var errorResponse = new ErrorResponse("An unexpected error occurred. Please try again later.");
        await response.WriteAsJsonAsync(errorResponse);
    }
}
