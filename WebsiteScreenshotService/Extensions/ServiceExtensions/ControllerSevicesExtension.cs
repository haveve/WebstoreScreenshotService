using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions
{
    public static class ControllerSevicesExtension
    {
        public static IMvcBuilder AddControllerServices(this IServiceCollection services)
        {
            return services.AddControllers()
                  .ConfigureApiBehaviorOptions(options =>
                  {
                      options.InvalidModelStateResponseFactory = context =>
                      {
                          var path = context.HttpContext.Request.Path;
                          return new BadRequestObjectResult(CreateValidationProblemDetails(context.ModelState, path));
                      };
                  });
        }

        public static ValidationProblemDetails CreateValidationProblemDetails(ModelStateDictionary modelState, string path)
        {
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
                Title = "One or more model validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "See the errors property for details",
                Instance = path
            };

            problemDetails.Extensions.Add("traceId", Guid.NewGuid());

            return problemDetails;
        }
    }
}
