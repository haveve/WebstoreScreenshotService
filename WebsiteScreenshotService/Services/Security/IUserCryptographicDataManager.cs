using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Security;

public interface IUserCryptographicDataManager
{
    Task<Result<EncryptionInfo>> GetUseCryptographicDataAsync(Guid userId);
}

public record EncryptionInfo(string Salt, string EncryptionKey);