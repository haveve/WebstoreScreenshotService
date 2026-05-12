using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Model;

/// <summary>
/// Represents the model for user registration.
/// </summary>
public class RegisterModel
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [EmailAddress]
    [Required]
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required]
    public string Password { get; set; } = default!;

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [Required]
    public string NickName { get; set; } = default!;

    /// <summary>
    /// Converts the <see cref="RegisterModel"/> to a <see cref="User"/> entity.
    /// </summary>
    /// <returns>A new <see cref="User"/> entity with the registration details.</returns>
    public UserCreateManagerModel ToEntity()
        => new(NickName, Email, Password, SubscriptionPlan.GetRegularSubscriptionPlan());
}

