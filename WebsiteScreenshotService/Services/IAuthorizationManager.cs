namespace WebsiteScreenshotService.Services;

public interface IAuthorizationManager
{
    public ConfirmationData? ValidateConfirmationToken(string token);

    public string? GenerateConfirmationToken(ConfirmationData confirmationData);
}

public record ConfirmationData(Guid UserId, string ScreenshotId, string WebsiteUrl);