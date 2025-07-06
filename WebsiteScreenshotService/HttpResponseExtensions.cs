namespace WebsiteScreenshotService;

public static class HttpResponseExtensions
{
    public static async Task UnauthorizedAccess(this HttpResponse response)
    {
        response.StatusCode = StatusCodes.Status401Unauthorized;
        var errorMessage = new ErrorResponse("Access denied due to invalid credentials.");
        await response.WriteAsJsonAsync(errorMessage);
    }
}
