using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions;

public static class SwaggerServicesExtension
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        return services.AddEndpointsApiExplorer()
            .AddSwaggerGen((options) =>
            {
                options.UseInlineDefinitionsForEnums();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Screenshot API",
                    Description = "Api for making screenshot of a website",
                    Contact = new OpenApiContact
                    {
                        Name = "Ivan Pohoidash",
                        Url = new Uri("https://github.com/haveve")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licence",
                        Url = new Uri("https://github.com/haveve/WebstoreScreenshotService/blob/main/LICENSE")
                    }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.ExampleFilters();
            })
            .AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
    }
}

