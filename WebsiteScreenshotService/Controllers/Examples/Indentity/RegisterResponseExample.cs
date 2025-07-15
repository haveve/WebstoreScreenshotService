using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using WebsiteScreenshotService.Extensions.ServiceExtensions;

namespace WebsiteScreenshotService.Controllers.Examples.Indentity;

public class LoginResponseExample : IMultipleExamplesProvider<string>
{
    public IEnumerable<SwaggerExample<string>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Validation Error",
            JsonSerializer.Serialize(ControllerSevicesExtension.CreateValidationProblemDetails([], "Identity/Login"))
        );

        yield return SwaggerExample.Create(
            "User doesn't exist",
            "User doesn't exist"
        );
    }
}
