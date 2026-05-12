using Shared.Core.Validation;
using Shared.Core.Validation.Rules;
using System.Text.RegularExpressions;

namespace WebsiteScreenshotService.Model.Validation.Validators;

public sealed class RegisterModelValidator : Validator<RegisterModel>
{
    public RegisterModelValidator()
    {
        RuleFor(nameof(RegisterModel.Email), x => x.Email)
            .Required("Email is required.")
            .MaxLen(254, "Email must not exceed 254 characters.")
            .Must(RegexPatterns.Email.IsMatch, $"Email is not valid.");

        RuleFor(nameof(RegisterModel.Password), x => x.Password)
            .Required("Password is required.")
            .MinLen(8, "Password must be at least 8 characters long.")
            .MaxLen(128, "Password must not exceed 128 characters.")
            .Must(HasRequiredComplexity,
                "Password must contain uppercase, lowercase, digit, and special character.");

        RuleFor(nameof(RegisterModel.NickName), x => x.NickName)
            .Required("NickName is required.")
            .MaxLen(50, "NickName must not exceed 50 characters.")
            .Must(RegexPatterns.NickName.IsMatch, $"NickName is not valid.");
    }

    private static bool HasRequiredComplexity(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c))
                hasUpper = true;
            else if (char.IsLower(c))
                hasLower = true;
            else if (char.IsDigit(c))
                hasDigit = true;
            else if (IsSpecialChar(c))
                hasSpecial = true;

            if (hasUpper && hasLower && hasDigit && hasSpecial)
                return true;
        }

        return false;
    }

    private static bool IsSpecialChar(char c)
        => !char.IsLetterOrDigit(c);
}
