using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenshotWorker.Services;
using ScreenshotWorker.Services.ContentInitialization;
using Microsoft.Extensions.Logging;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Repositories;
using ScreenshotWorker.Managers;
using ScreenshotWorker.Extensions;
using WebsiteScreenshotService;
using Microsoft.Extensions.Options;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
           {
               services.AddSingleton<IContentInitializationManager, ContentInitializationManager>();

               services.AddSingleton<IContentInitializationStep, ScrollToPageEndStep>();
               services.AddSingleton<IContentInitializationStep, WaitForRequestsToCompleteStep>();

               services.AddSingleton<IScreenshotService, ScreenshotService>();
               services.AddSingleton<IBrowserService, BrowserService>();

               services.AddSingleton<IApplicationLifetimeManager, ApplicationLifetimeManager>();

               services.AddOptionsWithValidation<BrowserServiceSettings>(context.Configuration.GetSection("BrowserServiceOptions"));
               services.AddOptionsWithValidation<MessageBrokerSettings>(context.Configuration.GetSection("MessageBrokerSettings"));
               services.AddOptionsWithValidation<ScreenshotServiceSettings>(context.Configuration.GetSection("ScreenshotServiceSettings"));

               if (context.HostingEnvironment.IsDevelopment())
               {
                   services.AddSingleton<IScreenshotRepository, LocalScreenshotRepository>();
                   services.AddOptionsWithValidation<LocalScreenshotStorageSettings>(context.Configuration.GetSection("ScreenshotStorageSettings"));
               }

               services.RegisterServiceRepositoryHttpClient();

               services.AddGrpcClient<GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceClient>((sp, o) =>
               {
                   var settings = sp.GetRequiredService<IOptions<ScreenshotServiceSettings>>().Value;
                   o.Address = new Uri(settings.Url);
               });
           });

builder.ConfigureLogging(logging =>
           {
               logging.ClearProviders()
               .AddConsole()
               .AddDebug();
           });

using var app = builder.Build();

using var screenshotWorkerApplication = app.Services.GetRequiredService<IApplicationLifetimeManager>();
await screenshotWorkerApplication.StartApplicationAsync();

await app.RunAsync(screenshotWorkerApplication.CancellationToken);