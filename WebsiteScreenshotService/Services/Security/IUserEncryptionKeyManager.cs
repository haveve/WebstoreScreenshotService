namespace WebsiteScreenshotService.Services.Security;

public interface IUserCryptographicDataManager
{
    Task<EncryptionInfo> GetUseCryptographicDataAsync(Guid userId);
}

public record EncryptionInfo(string Salt, string EncryptionKey);