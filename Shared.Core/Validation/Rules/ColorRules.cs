using System.Text.RegularExpressions;

namespace Shared.Core.Validation.Rules;

public static class ColorRules
{
    private static readonly Regex HexColorRegex =
        new(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$",
            RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public static RuleBuilder<T, string> SafeHexColor<T>(
        this RuleBuilder<T, string> rule)
    {
        return rule.Add((value, result, path) =>
        {
            if (value == null)
                return;

            // 1. CRLF injection protection
            if (value.IndexOf('\r') >= 0 || value.IndexOf('\n') >= 0)
            {
                result.Add(path, "Color contains invalid control characters.");
                return;
            }

            // 2. hex validation
            if (!HexColorRegex.IsMatch(value))
            {
                result.Add(path,
                    "Must be a valid hex color (#fff or #ffffff).");
            }
        });
    }
}
