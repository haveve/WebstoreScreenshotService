namespace WebsiteScreenshotService.Services.Security;

public interface IUserHashingService
{
    string Hash(string input);

    bool Verify(string input, string expectedHash);
}
