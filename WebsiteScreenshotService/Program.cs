using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using WebsiteScreenshotService;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.ServiceExtensions;
using WebsiteScreenshotService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddOptionsWithValidation<MessageBrokerConfigurations>(builder.Configuration.GetSection("MessageBrokerConfigurations"));

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
builder.Services.AddSingleton<IMessageBrokerProvider, MessageBrokerProvider>();

builder.Services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
builder.Services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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