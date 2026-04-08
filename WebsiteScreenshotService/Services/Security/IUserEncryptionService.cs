namespace WebsiteScreenshotService.Services.Security;

public interface IUserEncryptionService
{
    string Encrypt(string input);

    string Decrypt(string input);
}
