using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Core.Validation;
using WebsiteScreenshotService.Model.Validation;

namespace WebsiteScreenshotService.Controllers.Filters;

public class ValidationFilter(IValidatorRegistry registry) : IActionFilter
{
    private readonly IValidatorRegistry _registry = registry;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var finalResult = new ValidationResult();

        foreach (var (name, value) in context.ActionArguments)
        {
            if (value is null)
                continue;

            if (_registry.TryGet(value.GetType(), out var validator))
                finalResult.Merge(name, validator(value));
        }

        if (!finalResult.IsValid)
        {
            context.Result = new BadRequestObjectResult(new
            {
                finalResult.Errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}