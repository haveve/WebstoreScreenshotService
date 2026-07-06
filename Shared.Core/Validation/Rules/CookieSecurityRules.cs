using System.Text.RegularExpressions;

namespace Shared.Core.Validation.Rules;

public static class CookieSecurityRules
{
    public static RuleBuilder<T, string> SafeCookieString<T>(
        this RuleBuilder<T, string> rule,
        int maxLength = 200)
    {
        return rule.Add((value, result, path) =>
        {
            if (value == null)
                return;

            // 1. length check
            if (value.Length > maxLength)
            {
                result.Add(path, $"Value is too long (max {maxLength}).");
                return;
            }

            // 2. CRLF injection protection
            if (value.IndexOf('\r') >= 0 || value.IndexOf('\n') >= 0)
            {
                result.Add(path, "Value contains invalid control characters.");
                return;
            }
        });
    }

    private static readonly Regex DomainRegex = new(
        @"^(\.[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*)$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    public static RuleBuilder<T, string> SafeCookieDomain<T>(
        this RuleBuilder<T, string> rule,
        int maxLength = 200)
    {
        return rule
            .SafeCookieString(maxLength)
            .Add((value, result, path) =>
            {
                if (value == null)
                    return;

                if (!DomainRegex.IsMatch(value))
                    result.Add(path, "Invalid cookie domain format.");
            });
    }

    private static readonly Regex PathRegex = new(
        @"^\/[a-zA-Z0-9\/_\-\.]*$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100));

    public static RuleBuilder<T, string> SafeCookiePath<T>(
        this RuleBuilder<T, string> rule,
        int maxLength = 100)
    {
        return rule
            .SafeCookieString(maxLength)
            .Add((value, result, path) =>
            {
                if (value == null)
                    return;

                if (!PathRegex.IsMatch(value))
                    result.Add(path, "Invalid cookie path format.");
            });
    }
}