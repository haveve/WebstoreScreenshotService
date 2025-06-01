using Microsoft.Extensions.Options;

namespace WebsiteScreenshotService.ServiceExtensions;

public static class GeneralServiceExtensions
{
    public static OptionsBuilder<TOptions> AddOptionsWithValidation<TOptions>(this IServiceCollection services, IConfiguration config) where TOptions : class
    {
        return services.AddOptions<TOptions>()
            .Bind(config)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
