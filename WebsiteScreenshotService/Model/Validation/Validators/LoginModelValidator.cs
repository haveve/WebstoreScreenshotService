using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace WebsiteScreenshotService.Model.Validation.Validators;

public sealed class LoginModelValidator : Validator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(nameof(LoginModel.NickName), x => x.NickName)
            .Required("NickName is required.")
            .MaxLen(50, "NickName must not exceed 50 characters.")
            .Must(RegexPatterns.NickName.IsMatch, $"NickName is not valid.");

        RuleFor(nameof(LoginModel.Password), x => x.Password)
            .Required("Password is required.")
            .MinLen(12, "Password must be at least 12 characters long.")
            .MaxLen(128, "Password must not exceed 128 characters.");
    }
}