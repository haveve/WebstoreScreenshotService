using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebsiteScreenshotService.Repositories;
using Microsoft.AspNetCore.Authentication;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Entities;
using Swashbuckle.AspNetCore.Filters;
using WebsiteScreenshotService.Controllers.Examples.Indentity;

namespace WebsiteScreenshotService.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class IdentityController(IUserRepository UserRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = UserRepository;

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
    [Produces("application/json")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(LoginResponseExample))]
    public async Task<IActionResult> Login(LoginModel login)
    {
        var user = await _userRepository.GetUserByEmailAndPasswordAsync(login.Email, login.Password);

        if (user == null)
            return BadRequest("User doesn't exist");

        await AuthorizeAsync(user);

        return Ok(UserModel.GetModel(user));
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
    [Produces("application/json")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RegisterResponseExample))]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
        var user = await _userRepository.CreateUser(registerModel.ToEntity());

        if (user == null)
            return BadRequest("User with that email already exist");

        await AuthorizeAsync(user);

        return Ok(UserModel.GetModel(user));
    }

    /// <summary>
    /// Logs out the current user by signing them out of the system.
    /// </summary>
    /// <returns>
    /// A 200 OK response indicating successful logout.
    /// </returns>
    /// <response code="200">Successful logout.</response>
    [HttpGet]
    [Produces(typeof(void))]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }

    private async Task AuthorizeAsync(User user)
    {
        var claim = user.GetUserClaims();

        var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPricipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimsPricipal);
    }
}

