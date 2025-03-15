using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Entities;

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
    /// Gets or sets the surname of the user.
    /// </summary>
    [Required]
    public string Surname { get; set; } = default!;

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Converts the <see cref="RegisterModel"/> to a <see cref="User"/> entity.
    /// </summary>
    /// <returns>A new <see cref="User"/> entity with the registration details.</returns>
    public User ToEntity()
        => new(Guid.NewGuid(), Name, Surname, Email, Password, SubscriptionPlan.GetRegularSubscriptionPlan());
}

