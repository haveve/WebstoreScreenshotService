namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public class UserEntity
{
    public Guid Id { get; set; }

    public required string NickNameHash { get; set; }

    public required string PasswordHash { get; set; }

    public required string Salt { get; set; }

    public required string EncKey { get; set; }

    public required SubscriptionPlanValueObject SubscriptionPlan { get; set; }

    public required string EncryptedData { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLoginAt { get; set; }
};

public record UserEncryptedData(string NickName, string Email, TwoFactorModel? TwoFactorModel);

public record TwoFactorModel(string TotpSecret, IReadOnlyList<RecoveryCode> RecoveryCodes);

public record RecoveryCode(string Code, bool WasUsed);

