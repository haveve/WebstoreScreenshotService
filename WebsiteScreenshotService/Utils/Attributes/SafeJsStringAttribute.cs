using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebsiteScreenshotService.Utils.Attributes;

/// <summary>
/// Validates that a string is safe to inject into JS as plain text.
/// Allows all Unicode letters and digits, spaces, and basic punctuation.
/// Prevents XSS and JS injection.
/// </summary>
public class SafeJsStringAttribute : ValidationAttribute
{
    private readonly int _maxLength;
    private const int defaultTimeoutMilliseconds = 100;


    // Allow:
    // - \p{L} = all Unicode letters
    // - \p{N} = all Unicode numbers
    // - space, dash, underscore, common punctuation
    private static readonly Regex SafePattern = new Regex(
        @"^[\p{L}\p{N} _\-\.,!?;:()""']+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMicroseconds(defaultTimeoutMilliseconds));

    public SafeJsStringAttribute(int maxLength = 200)
    {
        _maxLength = maxLength;
        ErrorMessage = $"Input contains invalid characters or exceeds {_maxLength} chars.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        if (value is not string str) return new ValidationResult($"{validationContext.DisplayName} must be a string.");
        if (str.Length > _maxLength) return new ValidationResult(ErrorMessage);

        // CR/LF check to prevent header injection or JS breaking
        if (str.Contains('\r') || str.Contains('\n'))
            return new ValidationResult(ErrorMessage);

        if (!SafePattern.IsMatch(str))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}