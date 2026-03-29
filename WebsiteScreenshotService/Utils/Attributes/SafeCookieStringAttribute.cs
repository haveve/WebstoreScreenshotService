using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebsiteScreenshotService.Utils.Attributes;

/// <summary>
/// Validates strings for cookies: no CR/LF, max length, optional pattern.
/// Used to prevent cookie injection attacks (HTTP header/JS injection).
/// </summary>
public class SafeCookieStringAttribute : ValidationAttribute
{
    private readonly int _maxLength;
    private readonly Regex? _allowedPattern;
    private const int defaultTimeoutMilliseconds = 100;

    public SafeCookieStringAttribute(int maxLength = 200, string? allowedPattern = null)
    {
        _maxLength = maxLength;
        if (!string.IsNullOrEmpty(allowedPattern))
            _allowedPattern = new Regex(allowedPattern, RegexOptions.Compiled, TimeSpan.FromMicroseconds(defaultTimeoutMilliseconds));
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not string str)
            return new ValidationResult($"{validationContext.DisplayName} must be a string.");

        if (str.Length > _maxLength)
            return new ValidationResult($"{validationContext.DisplayName} is too long (max {_maxLength}).");

        // Prevent CR/LF injections
        if (str.Contains('\r') || str.Contains('\n'))
            return new ValidationResult($"{validationContext.DisplayName} contains invalid characters.");

        // Optional regex check
        if (_allowedPattern is not null && !_allowedPattern.IsMatch(str))
            return new ValidationResult($"{validationContext.DisplayName} contains invalid characters.");

        return ValidationResult.Success;
    }
}
