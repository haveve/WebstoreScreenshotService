namespace WebsiteScreenshotService.Services.Security;

public interface IKeyService
{
    string GenerateBase64Key();
    byte[] FromBase64(string base64Key);
    string ToBase64(byte[] keyBytes);
}