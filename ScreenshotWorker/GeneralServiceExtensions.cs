using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ScreenshotWorker;

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
