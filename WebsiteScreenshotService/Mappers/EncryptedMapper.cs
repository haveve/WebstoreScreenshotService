using System.Text.Json;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Mappers;

public abstract class EncryptedMapper(IUserContextAccessor userContextAccessor)
{
    protected readonly IUserEncryptionService encryptionService = userContextAccessor.GetUserSpecificServices().EncryptionService;

    protected string EncryptAsJson<T>(T value, UserEncryptionContext encryptionContext) where T : class
        => encryptionService.Encrypt(JsonSerializer.Serialize(value), encryptionContext);

    protected T Decrypt<T>(string encryptedValue, UserEncryptionContext encryptionContext) where T : class
        => TryDecrypt<T>(encryptedValue, encryptionContext) ?? throw new InvalidCastException("");

    protected T? TryDecrypt<T>(string encryptedValue, UserEncryptionContext encryptionContext) where T : class
        => JsonSerializer.Deserialize<T>(encryptionService.Decrypt(encryptedValue, encryptionContext));
}
