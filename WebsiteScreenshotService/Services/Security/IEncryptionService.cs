namespace WebsiteScreenshotService.Services.Security;

public interface IEncryptionService
{
    string Encrypt(string plaintext, byte[] key, EncryptionContext context);
    string Decrypt(string encrypted, byte[] key, EncryptionContext context);
}

public sealed record EncryptionContext(
    string TenantId,
    string Purpose);