using Microsoft.AspNetCore.Authentication.Cookies;
using PuppeteerSharp;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.ServiceExtensions;
using WebsiteScreenshotService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices();

if (builder.Environment.IsDevelopment())
    builder.Services.AddSwaggerServices();

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IBrowserService, BrowserService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
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

app.UseCors(builder => builder.WithOrigins("https://localhost:3000")
                 .AllowAnyHeader()
                 .WithMethods(["POST", "GET"])
                 .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();