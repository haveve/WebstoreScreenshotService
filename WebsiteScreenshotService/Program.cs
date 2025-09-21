using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ScreenshotWorker.Settings;
using System.Text.Json;
using WebsiteScreenshotService;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions.ServiceExtensions;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddGrpc();

builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Server"));
builder.Services.AddOptionsWithValidation<MessageBrokerConfigurations>(builder.Configuration.GetSection("MessageBroker"));
builder.Services.AddOptionsWithValidation<AuthorizationConfiguration>(builder.Configuration.GetSection("Authorization"));
builder.Services.AddOptionsWithValidation<ScreenshotStorageConfigurations>(builder.Configuration.GetSection("ScreenshotStorageSettings"));

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
builder.Services.AddSingleton<IAuthorizationManager, AuthorizationManager>();

builder.Services.AddSingleton<IMessageBrokerChannelManager, RabbitMqChannelManager>();
builder.Services.AddSingleton<IMessageBrokerManager, MessageBrokerManager>();

builder.Services.AddSingleton<ISubscriptionManager, InMemorySubscriptionManager>();
builder.Services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();

builder.Services.AddSingleton<IScreenshotManager, InMemoryScreenshotManager>();
builder.Services.AddSingleton<IScreenshotRepository, InMemoryScreenshotRepository>();

builder.Services.AddSingleton<IScreenshotStorageManager, ScreenshotStorageManager>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };

        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddSingleton<UserContextInitializeMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<GrpcScreenshotService>();

app.UseCors(builder => builder.WithOrigins(app.Configuration.GetValue<string>("FrontUrl")!)
                 .AllowAnyHeader()
                 .WithMethods(["POST", "GET"])
                 .AllowCredentials());

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserContextInitializeMiddleware>();

app.MapControllers();
app.Run();