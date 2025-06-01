using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.ServiceExtensions;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.ContentInitialization;
using WebsiteScreenshotService.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddOptionsWithValidation<BrowserServiceSettings>(builder.Configuration.GetSection("BrowserServiceOptions"));

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IBrowserService, BrowserService>();

builder.Services.AddSingleton<IContentInitializationManager, ContentInitializationManager>();
builder.Services.AddSingleton<IContentInitializationStep, ScrollToPageEndStep>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();