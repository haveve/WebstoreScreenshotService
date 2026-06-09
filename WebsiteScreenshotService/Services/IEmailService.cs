namespace WebsiteScreenshotService.Services;


public interface IEmailService
{
    Task SendResetPasswordEmailAsync(
        ResetPasswordEmail request,
        CancellationToken cancellationToken = default);

    Task SendRegisterFirstAdminEmailAsync(
        RegisterFirstAdminEmail request,
        CancellationToken cancellationToken = default);
}

public class RegisterFirstAdminEmail
{
    public string To { get; set; } = default!;

    public string Token { get; set; } = default!;
}

public class ResetPasswordEmail
{
    public string To { get; set; } = default!;

    public string Token { get; set; } = default!;
}