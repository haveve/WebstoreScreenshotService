using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using WebsiteScreenshotService.ServiceExtensions;

namespace WebsiteScreenshotService.Controllers.Examples.Indentity;

public class MakeScreenshotResponseExample : IMultipleExamplesProvider<string>
{
    public IEnumerable<SwaggerExample<string>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Validation Error",
            JsonSerializer.Serialize(ControllerSevicesExtension.CreateValidationProblemDetails([], "/MakeScreenshot"))
        );
        
        yield return SwaggerExample.Create(
            "User doesn't exist",
            "User doesn't exist"
        );

        yield return SwaggerExample.Create(
            "Limit of available screenshots exceeded",
            "You cannot make screenshot any more because you ran out of avaialbe screenshots"
        );
    }
}
