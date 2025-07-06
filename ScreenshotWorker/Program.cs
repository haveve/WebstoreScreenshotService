using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenshotWorker.Services;
using ScreenshotWorker.Services.ContentInitialization;
using ScreenshotWorker;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
           {
               services.AddSingleton<IContentInitializationManager, ContentInitializationManager>();

               services.AddSingleton<IContentInitializationStep, ScrollToPageEndStep>();
               services.AddSingleton<IContentInitializationStep, WaitForRequestsToCompleteStep>();

               services.AddSingleton<IBrowserService, BrowserService>();

               services.AddSingleton<IApplicationLifetimeManager, ApplicationLifetimeManager>();

               services.AddOptionsWithValidation<BrowserServiceSettings>(context.Configuration.GetSection("BrowserServiceOptions"));
               services.AddOptionsWithValidation<MessageBrokerConfigurations>(context.Configuration.GetSection("MessageBrokerConfigurations"));
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