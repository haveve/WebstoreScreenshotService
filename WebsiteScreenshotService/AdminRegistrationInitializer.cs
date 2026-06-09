using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Services;

namespace WebsiteScreenshotService;

public sealed class AdminRegistrationInitializer(IServiceScopeFactory scopeFactory) : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ScreenshotDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var authorizationManager = scope.ServiceProvider.GetRequiredService<IAuthorizationManager>();
        var configuration = scope.ServiceProvider.GetRequiredService<IOptions<InitializeAdminConfiguration>>();

        var adminExists = await db.Admins.AnyAsync(cancellationToken);

        if (adminExists)
            return;

        var token = authorizationManager.GenerateRegisterFirstAdminToken();

        if (token is null)
            throw new InvalidDataException("Was unable to generate admin creation token");

        await emailService.SendRegisterFirstAdminEmailAsync(
            new RegisterFirstAdminEmail
            {
                To = configuration.Value.Email,
                Token = token
            },
            cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
