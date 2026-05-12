namespace WebsiteScreenshotService.Services.Security;

public interface IUserEncryptionService
{
    string Encrypt(string input, UserEncryptionContext encryptionContext);

    string Decrypt(string input, UserEncryptionContext encryptionContext);
}

public record UserEncryptionContext(string Purpose);