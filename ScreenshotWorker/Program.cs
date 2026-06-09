using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScreenshotWorker.Extensions;
using ScreenshotWorker.Managers;
using ScreenshotWorker.Repositories;
using ScreenshotWorker.Services;
using ScreenshotWorker.Services.ContentInitialization;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Utils;
using Shared.Core.Contracts.ScreeshotModel.Validation;
using Shared.Core.Services;
using WebsiteScreenshotService;

var builder = Host.CreateDefaultBuilder(args);

builder
    .ConfigureAppConfiguration((hostingContext, config) =>
          {
              config.Sources.Clear();
              config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
              config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              config.AddEnvironmentVariables();
          })
    .ConfigureServices((context, services) =>
           {
               services.AddSingleton<MakeScreenshotModelValidator>();

               services.AddSingleton<IContentInitializationManager, ContentInitializationManager>();

               services.AddSingleton<IContentInitializationStep, ScrollToPageEndStep>();
               services.AddSingleton<IContentInitializationStep, WaitForRequestsToCompleteStep>();
               services.AddSingleton<IContentInitializationStep, WaitForSelectorStep>();
               services.AddSingleton<IContentInitializationStep, WaitForElementToAppearStep>();

               services.AddSingleton(new BrowserPool(maxContexts: 10, restartAfterJobs: 1000));

               services.AddSingleton<IScreenshotService, ScreenshotService>();
               services.AddSingleton<IBrowserService, BrowserService>();

               services.AddSingleton<IMessageBrokerManager, MessageBrokerManager>();

               services.AddSingleton<IApplicationLifetimeManager, ApplicationLifetimeManager>();

               services.AddOptionsWithValidation<BrowserServiceSettings>(context.Configuration.GetSection("BrowserServiceOptions"));
               services.AddOptionsWithValidation<MessageBrokerSettings>(context.Configuration.GetSection("MessageBrokerSettings"));
               services.AddOptionsWithValidation<ScreenshotServiceSettings>(context.Configuration.GetSection("ScreenshotServiceSettings"));
               
               if (context.HostingEnvironment.IsDevelopment())
               {
                   services.AddOptionsWithValidation<LocalScreenshotStorageSettings>(context.Configuration.GetSection("ScreenshotStorageSettings"));
                   services.AddSingleton<IScreenshotRepository, LocalScreenshotRepository>();
               }
               else
               {
                   services.AddOptionsWithValidation<BlobConfigurations>(context.Configuration.GetSection("Blob"));
                   services.AddSingleton<IScreenshotRepository, BlobScreenshotRepository>();
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
await app.StartApplicationAsync(args);