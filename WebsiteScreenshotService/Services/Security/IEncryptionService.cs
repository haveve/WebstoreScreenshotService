namespace WebsiteScreenshotService.Services.Security;

public interface IEncryptionService
{
    string Encrypt(string input, byte[] key);

    string Decrypt(string input, byte[] key);
}
