using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
using WebsiteScreenshotService.Repositories.TokenRepository;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Caching.Services;
using WebsiteScreenshotService.Services.Caching.Services.impl;
using WebsiteScreenshotService.Services.Messaging;
using WebsiteScreenshotService.Services.Payment;
using WebsiteScreenshotService.Services.Payment.Stripe;
using WebsiteScreenshotService.Services.Security;
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
builder.Services.AddOptionsWithValidation<HashingConfigurations>(builder.Configuration.GetSection("Hashing"));

builder.Services.AddSingleton<IKeyService, KeyService>();
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();
builder.Services.AddSingleton<IHashingService>(c => new Pbkdf2HashingService(c.GetService<IOptions<HashingConfigurations>>()!.Value!));

builder.Services.AddSingleton<ICacheGroupStateStore, InMemoryCacheGroupStateStore>();

builder.Services.AddSingleton<IUserCacheService, UserCacheService>();
builder.Services.AddSingleton<ICategoryCacheService, CategoryCacheService>();
builder.Services.AddSingleton<IScreenshotCacheService, ScreenshotCacheService>();
builder.Services.AddSingleton<IApiTokenCacheService, ApiTokenCacheService>();
builder.Services.AddSingleton<IRefreshTokenCacheService, RefreshTokenCacheService>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

builder.Services.AddMessageBrokerMassTransit();

builder.Services.AddSingleton<IPaymentProvider, StripePaymentProvider>();
builder.Services.AddSingleton<IPaymentProviderDataProcessor, StripePaymentProviderDataProcessor>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUserContextAccessor, UserContextAccessor>();

builder.Services.AddSingleton<ICategoryEntityMapper, CategoryEntityMapper>();
builder.Services.AddSingleton<IUserEntityMapper, UserEntityMapper>();
builder.Services.AddSingleton<IScreenshotEntityMapper, ScreenshotEntityMapper>();
builder.Services.AddSingleton<IApiTokenEntityMapper, ApiTokenEntityMapper>();
builder.Services.AddSingleton<IRefreshTokenEntityMapper, RefreshTokenEntityMapper>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddScoped<IUserCryptographicDataManager, UserCryptographicDataManager>();

builder.Services.AddSingleton<IScreenshotService, ScreenshotService>();

builder.Services.AddSingleton<IAuthorizationManager, AuthorizationManager>();

builder.Services.AddScoped<IMessageBrokerChannelManager, MassTransitChannelManager>();
builder.Services.AddScoped<IMessageBrokerManager, MessageBrokerManager>();

builder.Services.AddScoped<ISubscriptionManager, SubscriptionManager>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

builder.Services.AddScoped<IScreenshotManager, ScreenshotManager>();
builder.Services.AddScoped<IScreenshotRepository, ScreenshotRepository>();

builder.Services.AddScoped<ICategoryManager, CategoryManager>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenManager, TokenManager>();

builder.Services.AddSingleton<IScreenshotStorageManager, ScreenshotStorageManager>();

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Events = new CookieAuthenticationEvents
//        {
//            OnRedirectToLogin = context =>
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                return Task.CompletedTask;
//            }
//        };

//        options.SlidingExpiration = true;

//        if (builder.Environment.IsProduction())
//        {
//            options.Cookie.SameSite = SameSiteMode.None;
//            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        }
//        else
//        {
//            options.Cookie.SameSite = SameSiteMode.Lax;
//            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
//        }
//    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.User.FetchScreenshots, p =>
    {
        string[] permissions = [Permissions.User.FullAccess, Permissions.User.Screenshot.ManageScreenshots, Permissions.User.Screenshot.FetchScreenshots];
        p.Requirements.Add(new PermissionRequirements(permissions));
    });

    options.AddPolicy(Policies.User.MakeScreenshots, p =>
    {
        string[] permissions = [Permissions.User.FullAccess, Permissions.User.Screenshot.ManageScreenshots, Permissions.User.Screenshot.MakeScreenshots];
        p.Requirements.Add(new PermissionRequirements(permissions));
    });

    options.AddPolicy(Policies.User.ManageCategories, p =>
    {
        string[] permissions = [Permissions.User.FullAccess, Permissions.User.Category.ManageCategories];
        p.Requirements.Add(new PermissionRequirements(permissions));
    });
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

app.UseMiddleware<UserSpecificServicesInitializeMiddleware>();
app.UseMiddleware<UserContextInitializeMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.Run();

public record PermissionRequirements(string[] Permissions) : IAuthorizationRequirement;

public class PermissionHandler(IUserContextAccessor userContextAccessor) : AuthorizationHandler<PermissionRequirements>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirements requirement)
    {
        var currentUser = userContextAccessor.TryCurrentUser();

        if (currentUser is null)
            return;

        var currentUserPermissions = currentUser.UserInfo.Permissions;

        var hasPermission = requirement.Permissions.Any(rp => currentUserPermissions.Any(p => p == rp));

        if (hasPermission)
            context.Succeed(requirement);
    }
}