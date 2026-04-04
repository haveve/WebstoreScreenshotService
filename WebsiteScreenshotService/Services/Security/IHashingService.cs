namespace WebsiteScreenshotService.Services.Security;

public interface IHashingService
{
    string Hash(string input);

    bool Verify(string input, string expectedHash);

    string Hash(string input, string salt);
    
    bool Verify(string input, string salt, string expectedHash);
}
