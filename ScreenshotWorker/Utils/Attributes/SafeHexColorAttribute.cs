using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ScreenshotWorker.Utils.Attributes;

/// <summary>
/// Validates that a string is a safe hex color (#fff or #ffffff).
/// Prevents JS injection.
/// </summary>
public class SafeHexColorAttribute : ValidationAttribute
{
    private static readonly Regex HexColorRegex = new(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$", RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        if (value is not string str) return new ValidationResult($"{validationContext.DisplayName} must be a string.");

        // Prevent CR/LF injection
        if (str.Contains('\r') || str.Contains('\n'))
            return new ValidationResult($"{validationContext.DisplayName} contains invalid characters.");

        // Check hex pattern
        if (!HexColorRegex.IsMatch(str))
            return new ValidationResult($"{validationContext.DisplayName} must be a valid hex color (#fff or #ffffff).");

        return ValidationResult.Success;
    }
}
