using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using WebsiteScreenshotService.Controllers.Examples.Indentity;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories.UserRepository;

namespace WebsiteScreenshotService.Controllers;

[Route("identity/[action]")]
[ApiController]
public class IdentityController(IUserManager userManager) : ControllerBase
{
    private readonly IUserManager _userManager = userManager;

    /// <summary>
    /// Retrieves the information of the currently authenticated user.
    /// </summary>
    /// <returns>
    /// A UserModel with user details if the user is authenticated, or a BadRequest if the user does not exist.
    /// </returns>
    /// <response code="200">Returns the UserModel or null if used is not authorized or user does not exist.</response>
    [HttpGet]
    [ActionName("getUserInfo")]
    [Produces("application/json")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await _userManager.GetUser();

        if (!user.IsSuccess)
            return BadRequest("User doesn't exist");
        
        var userData = user.Value!;
        return Ok(UserModel.GetModel(userData));
    }

    /// <summary>
    /// Logs in a user using their email and password.
    /// </summary>
    /// <param name="login">The login details containing email and password.</param>
    /// <returns>
    /// A UserModel with user details if the login is successful, or a BadRequest if the user does not exist.
    /// </returns>
    /// <response code="200">Returns the UserModel if login is successful.</response>
    /// <response code="400">User doesn't exist or invalid credentials or Input data is invalid.</response>
    [HttpPost]
    [ActionName("login")]
    [Produces("application/json")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(LoginResponseExample))]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var user = await _userManager.GetUserByNickNameAndPasswordAsync(login.NickName, login.Password);

        if (!user.IsSuccess)
            return BadRequest("User doesn't exist");

        var userData = user.Value!;
        await AuthorizeAsync(userData);

        return Ok(UserModel.GetModel(userData));
    }

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="registerModel">The registration details containing email, password, etc.</param>
    /// <returns>
    /// A 200 OK response if registration is successful, or a 400 BadRequest if a user with the same email already exists.
    /// </returns>
    /// <response code="200">Successful registration of the user.</response>
    /// <response code="400">User with that email already exists or Input data is invalid.</response>
    [HttpPost]
    [ActionName("register")]
    [Produces("application/json")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RegisterResponseExample))]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        var user = await _userManager.CreateUserAsync(registerModel.ToEntity());

        if (!user.IsSuccess)
            return BadRequest("User with that email already exist");

        var userData = user.Value!;

        await AuthorizeAsync(userData);

        return Ok(UserModel.GetModel(userData));
    }

    /// <summary>
    /// Logs out the current user by signing them out of the system.
    /// </summary>
    /// <returns>
    /// A 200 OK response indicating successful logout.
    /// </returns>
    /// <response code="200">Successful logout.</response>
    [HttpGet]
    [ActionName("logout")]
    [Produces(typeof(void))]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    private async Task AuthorizeAsync(User user)
    {
        var claim = user.GetUserClaims();

        var claimsIdentity = new ClaimsIdentity(claim, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimsPrincipal);
    }
}

