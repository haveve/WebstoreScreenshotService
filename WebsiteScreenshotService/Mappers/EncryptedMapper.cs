using System.Text.Json;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Mappers;

public abstract class EncryptedMapper(IUserContextAccessor userContextAccessor)
{
    protected readonly IUserEncryptionService encryptionService = userContextAccessor.GetUserSpecificServices().EncryptionService;

    protected string EncryptAsJson<T>(T value) where T : class
        => encryptionService.Encrypt(JsonSerializer.Serialize(value));

    protected T Decrypt<T>(string encryptedValue) where T : class
        => TryDecrypt<T>(encryptedValue) ?? throw new InvalidCastException("");

    protected T? TryDecrypt<T>(string encryptedValue) where T : class
        => JsonSerializer.Deserialize<T>(encryptionService.Decrypt(encryptedValue));
}
