namespace WebsiteScreenshotService.Services;

public interface IAuthorizationManager
{
    public Task<ConfirmationData?> ValidateConfirmationToken(string token);

    public Task<ResetPassword?> ValidateResetPasswordToken(string token);

    public Task ValidateRegisterFirstAdminToken(string token);

    public Task<RefreshData?> ValidateRefreshToken(string token);

    public Task<AccessData?> ValidateAccessToken(string token);

    public Task<ApiData?> ValidateApiToken(string token);

    public GeneratedTokenData? GenerateConfirmationToken(ConfirmationData confirmationData);

    public GeneratedTokenData? GenerateResetPasswordToken(ResetPassword data);

    public GeneratedTokenData? GenerateRegisterFirstAdminToken();

    public GeneratedTokenData? GenerateRefreshToken(RefreshData data);

    public GeneratedTokenData? GenerateAccessToken(AccessData data);

    public GeneratedTokenData? GenerateApiToken(ApiData data);
}

public record GeneratedTokenData(string Token, DateTime ExpiresOn);

public record RefreshData(Guid UserId);

public record AccessData(Guid UserId);

public record ApiData(Guid UserId, string[] Permissions, DateTime? ExpiresOn);

public record ResetPassword(Guid UserId);

public record ConfirmationData(Guid UserId, string TokenId, string ScreenshotId, int PointsCost);