using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using WebsiteScreenshotService.Extensions.ServiceExtensions;

namespace WebsiteScreenshotService.Controllers.Examples.Indentity;

public class RegisterResponseExample : IMultipleExamplesProvider<string>
{
    public IEnumerable<SwaggerExample<string>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Validation Error",
            JsonSerializer.Serialize(ControllerSevicesExtension.CreateValidationProblemDetails([], "Identity/Register"))
        );

        yield return SwaggerExample.Create(
            "User already exists",
            "User with that email already exist"
        );
    }
}
