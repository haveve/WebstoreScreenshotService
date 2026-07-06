using Microsoft.Extensions.DependencyInjection;

namespace ScreenshotWorker.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient GetServiceRepositoryHttpClient(this IHttpClientFactory httpClientFactory)
        => httpClientFactory.CreateClient("ServiceRepository");

    public static void RegisterServiceRepositoryHttpClient(this IServiceCollection services)
        => services.AddHttpClient("ServiceRepository");
}

