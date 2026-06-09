using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Core.Services;
using System.Text.Json;
using WebsiteScreenshotService;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions.ServiceExtensions;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Mappers.EntityMappers.impl;
using WebsiteScreenshotService.Model.Validation;
using WebsiteScreenshotService.Model.Validation.Validators;
using WebsiteScreenshotService.Repositories.AdminRepository;
using WebsiteScreenshotService.Repositories.CategoryRepository;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.ProductRepository;
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
using WebsiteScreenshotService.Services.Checkout;
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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IScreenshotSearchStrategy, SqliteScreenshotSearchStrategy>();
    builder.Services.AddDbContext<ScreenshotDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Dev")));
}
else
{
    builder.Services.AddSingleton<IScreenshotSearchStrategy, PostgresScreenshotSearchStrategy>();
    builder.Services.AddDbContext<ScreenshotDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Live")));
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
builder.Services.AddOptionsWithValidation<EncryptionConfigurations>(builder.Configuration.GetSection("MessageBroker"));
builder.Services.AddOptionsWithValidation<HashingConfigurations>(builder.Configuration.GetSection("Hashing"));
builder.Services.AddOptionsWithValidation<EmailConfigurations>(builder.Configuration.GetSection("Email"));
builder.Services.AddOptionsWithValidation<FrontendConfigurations>(builder.Configuration.GetSection("FrontendSettings"));
builder.Services.AddOptionsWithValidation<InitializeAdminConfiguration>(builder.Configuration.GetSection("InitializeAdmin"));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOptionsWithValidation<ScreenshotStorageConfigurations>(builder.Configuration.GetSection("ScreenshotStorageSettings"));
}
else
{
    builder.Services.AddOptionsWithValidation<BlobConfigurations>(builder.Configuration.GetSection("Blob"));
}

builder.Services.AddSingleton<IPaymentProviderCallbackConfigurationManager, PaymentProviderCallbackConfigurationManager>();
builder.Services.AddSingleton<IPaymentProviderConfigurationManager, PaymentProviderConfigurationManager>();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IKeyService, KeyService>();
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();
builder.Services.AddSingleton<IHashingService>(c => new Pbkdf2HashingService(c.GetService<IOptions<HashingConfigurations>>()!.Value!));

builder.Services.AddSingleton<IScreenshotCalculator, ScreenshotPricingCalculator>();
builder.Services.AddSingleton<ICacheGroupStateStore, InMemoryCacheGroupStateStore>();

builder.Services.AddSingleton<IUserCacheService, UserCacheService>();
builder.Services.AddSingleton<ICategoryCacheService, CategoryCacheService>();
builder.Services.AddSingleton<IScreenshotCacheService, ScreenshotCacheService>();
builder.Services.AddSingleton<IApiTokenCacheService, ApiTokenCacheService>();
builder.Services.AddSingleton<IRefreshTokenCacheService, RefreshTokenCacheService>();
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

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminManager, AdminManager>();

builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IProductPriceManager, ProductPriceManager>();

builder.Services.AddScoped<ICheckoutManager, CheckoutManager>();

if (builder.Environment.IsDevelopment())
    builder.Services.AddSingleton<IScreenshotStorageManager, ScreenshotStorageManager>();
else
    builder.Services.AddSingleton<IScreenshotStorageManager, BlobScreenshotStorageManager>();

builder.Services.AddSingleton<IEmailService, AzureEmailService>();

builder.Services.AddHostedService<AdminRegistrationInitializer>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddJwtAuthorization();

builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddSingleton<UserContextInitializeMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<GrpcScreenshotService>();

app.UseCors(builder => builder.WithOrigins(app.Configuration.GetValue<string>("FrontendSettings:FrontUrl")!)
                 .AllowAnyHeader()
                 .WithMethods(["POST", "GET"])
                 .AllowCredentials());

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UsePaymentCallback();

app.UseAuthentication();

app.UseMiddleware<UserSpecificServicesInitializeMiddleware>();
app.UseMiddleware<UserContextInitializeMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.Run();