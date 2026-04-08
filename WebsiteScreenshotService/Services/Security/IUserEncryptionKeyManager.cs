namespace WebsiteScreenshotService.Services.Security;

public interface IUserEncryptionKeyManager
{
    Task<string> GetUserEncryptionKeyAsync(Guid userId);
}