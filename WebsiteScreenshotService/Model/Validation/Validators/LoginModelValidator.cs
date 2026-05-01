using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace WebsiteScreenshotService.Model.Validation.Validators;

public sealed class LoginModelValidator : Validator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(nameof(LoginModel.Email), x => x.Email)
            .Required("Email is required.")
            .MaxLen(254, "Email must not exceed 254 characters.")
            .Must(RegexPatterns.Email.IsMatch, $"Email is not a valid email address.");

        RuleFor(nameof(LoginModel.Password), x => x.Password)
            .Required("Password is required.")
            .MinLen(12, "Password must be at least 12 characters long.")
            .MaxLen(128, "Password must not exceed 128 characters.");
    }
}