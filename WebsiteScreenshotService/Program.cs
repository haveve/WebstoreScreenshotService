using Microsoft.AspNetCore.Authentication.Cookies;
using PuppeteerSharp;
using System.Text.Json;
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

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IBrowserService, BrowserService>();

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

app.Lifetime.ApplicationStarted.Register(async () =>
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.WithOrigins(app.Configuration.GetValue<string>("FrontUrl") ?? "http://localhost:3000")
                 .AllowAnyHeader()
                 .WithMethods(["POST", "GET"])
                 .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();