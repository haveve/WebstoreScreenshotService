namespace WebsiteScreenshotService.Services.Security;

public interface IEncryptionService
{
    string Encrypt(string input);

    string Decrypt(string input);

    string Encrypt(string input, byte[] key);

    string Decrypt(string input, byte[] key);
}
