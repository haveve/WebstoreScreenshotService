using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.AdminRepository;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.TokenRepository;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Security;

[Route("identity/[action]")]
[ApiController]
public class IdentityController(
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

    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUser();

        if (!user.IsSuccess)
            return BadRequest("User doesn't exist");

        return Ok(UserModel.GetModel(user.Value!));
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var user = await _userManager
            .GetUserByNickNameAndPasswordAsync(login.NickName, login.Password);

        if (!user.IsSuccess)
            return Unauthorized();

        var userId = user.Value!.Id;

        var accessToken = _authorizationManager.GenerateAccessToken(
            new AccessData(userId));

        var refreshTokenValue = _authorizationManager.GenerateRefreshToken(
            new RefreshData(userId));

        if (refreshTokenValue is null)
            return BadRequest("Cannot authorize");

        var refreshTokenHash = _hashingService.Hash(refreshTokenValue);

        await _tokenManager.CreateRefreshTokenAsync(
            new CreateRefreshTokenModel(
                TokenHash: refreshTokenHash,
                FamilyId: Guid.CreateVersion7().ToString(),
                Expires: DateTime.UtcNow.AddDays(30),
                IssuerLocation: GetTokenLocation()
            ),
            userId);

        Response.Cookies.Append(
            "refresh_token",
            refreshTokenValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

        return Ok(new { AccessToken = accessToken });
    }

    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
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

        var newRefreshValue =
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var newRefreshHash = _hashingService.Hash(newRefreshValue);

        await _tokenManager.RevokeRefreshTokenAsync(
            new RevokeRefreshTokenModel(null, token.TokenHash, "Rotated"));

        await _tokenManager.CreateRefreshTokenAsync(
            new CreateRefreshTokenModel(
                newRefreshHash,
                token.FamilyId,
                DateTime.UtcNow.AddDays(30),
                token.TokenMetadata.IssuedLocation),
            token.UserId);

        Response.Cookies.Append(
            "refresh_token",
            newRefreshValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

        return Ok(new { AccessToken = newAccessToken });
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
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            var hash = _hashingService.Hash(refreshToken);

            await _tokenManager.RevokeRefreshTokenAsync(
                new RevokeRefreshTokenModel(null, hash, "Logout"));
        }

        Response.Cookies.Delete("refresh_token");

        return Ok();
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

        await _emailService.SendResetPasswordEmailAsync(new ResetPasswordEmail
        {
            To = user.Value.Email,
            Token = token!
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
    public async Task<IActionResult> RequestFirstAdmin([FromBody] string email)
    {
        await _adminInitLock.WaitAsync();

        try
        {
            var exists = await _adminManager.DoesAnyAdminExistAsync();

            if (exists.Value)
                return BadRequest("Admin already exists");

            var token = _authorizationManager.GenerateRegisterFirstAdminToken();

            await _emailService.SendRegisterFirstAdminEmailAsync(
                new RegisterFirstAdminEmail
                {
                    To = email,
                    Token = token!
                });

            return Ok();
        }
        finally
        {
            _adminInitLock.Release();
        }
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

    [HttpPost]
    public async Task<IActionResult> CreateApiKey(ApiKeyRequest model)
    {
        var result = await _tokenManager.CreateApiTokenAsync(
            new CreateApiTokenManagerModel(
                model.Name,
                TokenHash: Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                model.Expires,
                model.Scopes,
                model.AllowedIps,
                model.IssuerLocation));

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> RevokeApiKey(ApiKeyRevokeRequest model)
    {
        var result = await _tokenManager.RevokeApiTokenAsync(
            new RevokeApiTokenModel(model.TokenHash, model.Reason));

        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetApiKeys()
    {
        var userIdResult = await _userManager.GetUser();

        if (!userIdResult.IsSuccess)
            return Unauthorized();

        var userId = userIdResult.Value!.Id;

        var result = await _tokenManager.GetAllApiTokensAsync(userId);

        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Value);
    }

    private static TokenLocation GetTokenLocation()
        => new("UA", "Ukraine", "Zhytomyr");
}

public record LoginModel(string NickName, string Password);

public record RegisterModel(string NickName, string Email, string Password);

public record ResetPasswordRequest(string Token, string NewPassword);

public record CreateFirstAdminModel(
    string Token,
    AdminCreateModel Admin);

public record AdminCreateModel(
    string NickName,
    string Email,
    string Password);

public record ApiKeyRequest(
    string Name,
    ICollection<string> Scopes,
    ICollection<string> AllowedIps,
    DateTime Expires,
    TokenLocation IssuerLocation);

public record ApiKeyRevokeRequest(string TokenHash, string Reason);