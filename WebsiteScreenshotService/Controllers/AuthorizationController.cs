using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.AdminRepository;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.TokenRepository;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Controllers;

[Route("authorization/[action]")]
[ApiController]
public class AuthorizationController(
    IUserManager userManager,
    IAdminManager adminManager,
    IAuthorizationManager authorizationManager,
    ITokenManager tokenManager,
    IEmailService emailService,
    ILogger<IdentityController> logger,
    IHashingService hashingService
) : ControllerBase
{
    private static readonly SemaphoreSlim _adminInitLock = new(1, 1);

    private readonly IUserManager _userManager = userManager;
    private readonly IAdminManager _adminManager = adminManager;
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<IdentityController> _logger = logger;
    private readonly IHashingService _hashingService = hashingService;


    [HttpPost]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var user = await _userManager
            .GetUserByNickNameAndPasswordAsync(login.NickName, login.Password);

        if (!user.IsSuccess)
            return Unauthorized();

        if (user.Value!.IsDisactivated)
            return BadRequest(new ErrorResponse("User is deactivated"));

        var userId = user.Value!.Id;

        var accessToken = _authorizationManager.GenerateAccessToken(
            new AccessData(userId));

        var refreshTokenValue = _authorizationManager.GenerateRefreshToken(
            new RefreshData(userId));

        if (refreshTokenValue is null || accessToken is null)
            return BadRequest("Cannot authorize");

        var tokenData = refreshTokenValue.Token;
        var refreshTokenHash = _hashingService.Hash(tokenData);

        await _tokenManager.CreateRefreshTokenAsync(
            new CreateRefreshTokenModel(
                TokenHash: refreshTokenHash,
                FamilyId: Guid.CreateVersion7().ToString(),
                Expires: refreshTokenValue.ExpiresOn,
                IssuerLocation: GetTokenLocation()
            ),
            userId);

        Response.Cookies.Append(
            "refresh_token",
            tokenData,
            new CookieOptions
            {
                Path = "/authorization/refresh",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenValue.ExpiresOn
            });

        return Ok(new { AccessToken = accessToken.Token, Expires = accessToken.ExpiresOn, UserData = user.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            return Unauthorized();

        if (await _authorizationManager.ValidateRefreshToken(refreshToken) is null)
            return Unauthorized();

        var refreshHash = _hashingService.Hash(refreshToken);
        var storedToken = await _tokenManager.GetRefreshTokenByHashAsync(refreshHash);

        if (!storedToken.IsSuccess)
            return Unauthorized();

        var token = storedToken.Value!;

        if (token.Expires < DateTime.UtcNow || token.RevokedReason is not null)
            return Unauthorized();

        var newAccessToken = _authorizationManager.GenerateAccessToken(
            new AccessData(token.UserId));

        var refreshTokenValue = _authorizationManager.GenerateRefreshToken(
            new RefreshData(token.UserId));

        if (refreshTokenValue is null || newAccessToken is null)
            return BadRequest("Cannot authorize");

        var newRefreshHash = _hashingService.Hash(refreshTokenValue.Token);

        await _tokenManager.RevokeRefreshTokenAsync(
            new RevokeRefreshTokenModel(null, token.TokenHash, "Rotated"));

        await _tokenManager.CreateRefreshTokenAsync(
            new CreateRefreshTokenModel(
                newRefreshHash,
                token.FamilyId,
                refreshTokenValue.ExpiresOn,
                token.TokenMetadata.IssuedLocation),
            token.UserId);

        Response.Cookies.Append(
            "refresh_token",
            refreshTokenValue.Token,
            new CookieOptions
            {
                Path = "/authorization/refresh",
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenValue.ExpiresOn
            });

        return Ok(new { AccessToken = newAccessToken.Token, Expires = newAccessToken.ExpiresOn });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var user = await _userManager.CreateUserAsync(
            new UserCreateManagerModel(
                model.NickName,
                model.Email,
                model.Password,
                SubscriptionPlan.GetRegularSubscriptionPlan()));

        if (!user.IsSuccess)
            return BadRequest(user.ErrorMessage);

        return Ok(UserModel.GetModel(user.Value!));
    }

    [HttpPost]
    public async Task<IActionResult> RequestUserPasswordReset([FromBody] string nickName)
    {
        var user = await _userManager
            .GetUserByNickNameAsync(nickName);

        if (!user.IsSuccess)
            return BadRequest("User not found");

        var token = _authorizationManager.GenerateResetPasswordToken(
            new ResetPassword(user.Value!.Id));

        if (token is null)
            return BadRequest("User not found");

        await _emailService.SendResetPasswordEmailAsync(new ResetPasswordEmail
        {
            To = user.Value.Email,
            Token = token.Token!
        });

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ResetUserPassword(ResetPasswordRequest model)
    {
        var validated =
            await _authorizationManager.ValidateResetPasswordToken(model.Token);

        if (validated is null)
            return BadRequest("Invalid token");

        var result = await _userManager.UpdateUserPasswordAsync(
            new UserPasswordUpdateManagerModel(model.NewPassword),
            validated.UserId);

        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFirstAdmin(CreateFirstAdminModel model)
    {
        var exists = await _adminManager.DoesAnyAdminExistAsync();

        if (exists.Value)
            return BadRequest("Admin already exists");

        await _adminInitLock.WaitAsync();

        try
        {
            exists = await _adminManager.DoesAnyAdminExistAsync();

            if (exists.Value)
                return BadRequest("Admin already exists");

            await _authorizationManager
                .ValidateRegisterFirstAdminToken(model.Token);

            var result = await _adminManager.CreateAdminAsync(
                new AdminCreateManagerModel(
                    model.Admin.NickName,
                    model.Admin.Email,
                    model.Admin.Password));

            return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
        }
        catch
        {
            return BadRequest("Invalid token");
        }
        finally
        {
            _adminInitLock.Release();
        }
    }

    private static TokenLocation GetTokenLocation()
        => new("UA", "Ukraine", "Zhytomyr");
}
