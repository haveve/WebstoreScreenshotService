namespace WebsiteScreenshotService.Services.Security;

public class UserHashingService(IHashingService hashingService, string salt) : IUserHashingService
{
    public string Hash(string input)
        => hashingService.Hash(input, salt);

    public bool Verify(string input, string expectedHash)
        => hashingService.Verify(input, salt, expectedHash);

}
