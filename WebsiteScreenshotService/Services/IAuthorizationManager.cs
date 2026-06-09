namespace WebsiteScreenshotService.Services;

public interface IAuthorizationManager
{
    public Task<ConfirmationData?> ValidateConfirmationToken(string token);

    public Task<ResetPassword?> ValidateResetPasswordToken(string token);

    public Task ValidateRegisterFirstAdminToken(string token);

    public Task<RefreshData?> ValidateRefreshToken(string token);

    public Task<AccessData?> ValidateAccessToken(string token);

    public Task<ApiData?> ValidateApiToken(string token);

    public string? GenerateConfirmationToken(ConfirmationData confirmationData);

    public string? GenerateResetPasswordToken(ResetPassword data);

    public string? GenerateRegisterFirstAdminToken();

    public string? GenerateRefreshToken(RefreshData data);

    public string? GenerateAccessToken(AccessData data);

    public string? GenerateApiToken(ApiData data);
}

public record RefreshData(Guid UserId);

public record AccessData(Guid UserId);

public record ApiData(Guid UserId, string[] Permissions);

public record ResetPassword(Guid UserId);

public record ConfirmationData(Guid UserId, string TokenId, string ScreenshotId, int PointsCost);