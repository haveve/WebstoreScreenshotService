using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using WebsiteScreenshotService;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.TokenRepository;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Services;
using WebsiteScreenshotService.Services.Security;

[Authorize(Roles = UserRoles.User)]
[Route("identity/[action]")]
[ApiController]
public class IdentityController(
    IUserManager userManager,
    ITokenManager tokenManager,
    ILogger<IdentityController> logger,
    IHashingService hashingService,
    IAuthorizationManager authorizationManager,
    IUserContextAccessor userContextAccessor
) : ControllerBase
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly ILogger<IdentityController> _logger = logger;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;

    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUser();

        if (!user.IsSuccess)
            return BadRequest("User doesn't exist");

        return Ok(UserModel.GetModel(user.Value!));
    }

    [HttpPost]
    public async Task<IActionResult> CreateApiKey(ApiKeyRequest model)
    {
        var userId = _userContextAccessor.GetCurrentUser().UserInfo.Id;
        var apiToken = _authorizationManager.GenerateApiToken(new ApiData(userId, [.. model.Scopes], model.Expires));
        
        if(apiToken is null)
            return BadRequest("Cannot generate Api token");

        var hash = _hashingService.Hash(apiToken.Token);

        var result = await _tokenManager.CreateApiTokenAsync(
            new CreateApiTokenManagerModel(
                model.Name,
                TokenHash: hash,
                model.Expires,
                model.Scopes,
                model.AllowedIps,
                GetTokenLocation()));

        return result.IsSuccess ? Ok(apiToken) : BadRequest(result.ErrorMessage);
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
    DateTime Expires);

public record ApiKeyRevokeRequest(string TokenHash, string Reason);