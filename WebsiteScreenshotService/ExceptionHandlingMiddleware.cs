namespace WebsiteScreenshotService;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var errorResponse = new ErrorResponse("An unexpected error occurred. Please try again later.");

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}