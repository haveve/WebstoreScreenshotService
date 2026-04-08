using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Services.Security;

public class UserEncryptionService(IKeyService keyService, IEncryptionService encryptionService, EncryptionConfigurations settings, string key) : IUserEncryptionService
{
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly Lazy<byte[]> _lazyKey = new(() =>
        {
            var masterKey = keyService.FromBase64(settings.MasterKey);
            var decryptedKey = encryptionService.Decrypt(key, masterKey);
            return keyService.FromBase64(decryptedKey);
        }, LazyThreadSafetyMode.PublicationOnly);

    public string Encrypt(string input)
        => _encryptionService.Encrypt(input, _lazyKey.Value);

    public string Decrypt(string input)
        => _encryptionService.Decrypt(input, _lazyKey.Value);
}
