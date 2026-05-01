using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebsiteScreenshotService;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions.ServiceExtensions;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Mappers.EntityMappers.impl;
using WebsiteScreenshotService.Model.Validation;
using WebsiteScreenshotService.Model.Validation.Validators;
using WebsiteScreenshotService.Repositories.CategoryRepository;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Search;
using WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Messaging;
using WebsiteScreenshotService.Services.Payment;
using WebsiteScreenshotService.Services.Payment.Stripe;
using WebsiteScreenshotService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<ICacheManager, CacheManager>();

var databaseProvider = builder.Environment.IsProduction()
    ? Provider.Postgres
    : Provider.Sqlite;

builder.Services.AddSingleton(new StorageConfigurations() { Provider = databaseProvider });

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IScreenshotSearchStrategy, SqliteScreenshotSearchStrategy>();
    builder.Services.AddDbContext<ScreenshotDbContext>(options =>
        options.UseSqlite("Data Source=screenshots.db"));
}
else
{
    builder.Services.AddSingleton<IScreenshotSearchStrategy, PostgresScreenshotSearchStrategy>();
    builder.Services.AddDbContext<ScreenshotDbContext>(options =>
        options.UseNpgsql("host=localhost port=5432 dbname=mydb user=myuser password=mypassword"));
}

builder.Services.AddGrpc();

builder.Services.AddSingleton<IValidatorRegistry>(sp =>
{
    var registry = new ValidatorRegistry();

    registry.Add(new InputScreenshotModelValidator());
    registry.Add(new LoginModelValidator());
    registry.Add(new RegisterModelValidator());
    registry.Add(new PagingValidator());

    return registry;
});

builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Server"));
builder.Services.AddOptionsWithValidation<MessageBrokerConfigurations>(builder.Configuration.GetSection("MessageBroker"));
builder.Services.AddOptionsWithValidation<AuthorizationConfiguration>(builder.Configuration.GetSection("Authorization"));
builder.Services.AddOptionsWithValidation<ScreenshotStorageConfigurations>(builder.Configuration.GetSection("ScreenshotStorageSettings"));
builder.Services.AddOptionsWithValidation<EncryptionConfigurations>(builder.Configuration.GetSection("MessageBroker"));

builder.Services.AddMessageBrokerMassTransit();

builder.Services.AddSingleton<IPaymentProvider, StripePaymentProvider>();
builder.Services.AddSingleton<IPaymentProviderDataProcessor, StripePaymentProviderDataProcessor>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ICategoryEntityMapper, CategoryEntityMapper>();
builder.Services.AddSingleton<IUserEntityMapper, UserEntityMapper>();
builder.Services.AddSingleton<IScreenshotEntityMapper, ScreenshotEntityMapper>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddSingleton<IScreenshotService, ScreenshotService>();

builder.Services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
builder.Services.AddSingleton<IAuthorizationManager, AuthorizationManager>();

builder.Services.AddScoped<IMessageBrokerChannelManager, MassTransitChannelManager>();
builder.Services.AddScoped<IMessageBrokerManager, MessageBrokerManager>();

builder.Services.AddScoped<ISubscriptionManager, SubscriptionManager>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

builder.Services.AddScoped<IScreenshotManager, ScreenshotManager>();
builder.Services.AddScoped<IScreenshotRepository, ScreenshotRepository>();

builder.Services.AddScoped<ICategoryManager, CategoryManager>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

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

app.UseMiddleware<UserSpecificServicesInitializeMiddleware>();
app.UseMiddleware<UserContextInitializeMiddleware>();

app.MapControllers();
app.Run();