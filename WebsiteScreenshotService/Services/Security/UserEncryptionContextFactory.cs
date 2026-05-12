namespace WebsiteScreenshotService.Services.Security;

public static class UserEncryptionContextFactory
{
    public static UserEncryptionContext ApiTokenEncEntityData { get; } = new("api-token-enc-entity-data");

    public static UserEncryptionContext UserEncEntityData { get; } = new("user-enc-entity-data");

    public static EncryptionContext ForUserEncKey(string userId)
        => new(TenantId: userId.ToString(), Purpose: "user-enc-key");

    public static EncryptionContext ForUserEncEntityData(string userId)
        => new(TenantId: userId.ToString(), Purpose: UserEncEntityData.Purpose);
}
