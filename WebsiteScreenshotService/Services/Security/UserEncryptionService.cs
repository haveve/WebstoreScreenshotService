using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Services.Security;

public class UserEncryptionService(IKeyService keyService, IEncryptionService encryptionService, EncryptionConfigurations settings, string key, Guid userId) : IUserEncryptionService
{
    private readonly string _userId = userId.ToString();
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly Lazy<byte[]> _lazyKey = new(() =>
        {
            var masterKey = keyService.FromBase64(settings.MasterKey);
            var decryptedKey = encryptionService.Decrypt(key, masterKey, UserEncryptionContextFactory.ForUserEncKey(userId.ToString()));
            return keyService.FromBase64(decryptedKey);
        }, LazyThreadSafetyMode.PublicationOnly);

    public string Encrypt(string input, UserEncryptionContext encryptionContext)
        => _encryptionService.Encrypt(input, _lazyKey.Value, new(_userId, encryptionContext.Purpose));

    public string Decrypt(string input, UserEncryptionContext encryptionContext)
        => _encryptionService.Decrypt(input, _lazyKey.Value, new(_userId, encryptionContext.Purpose));
}
