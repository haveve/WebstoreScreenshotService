using System.Text.RegularExpressions;

namespace Shared.Core.Validation.Rules;

public static class JsStringRules
{
    private static readonly Regex SafePattern = new(
        @"^[\p{L}\p{N} _\-\.,!?;:()""']+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

    public static RuleBuilder<T, string> SafeJsString<T>(
        this RuleBuilder<T, string> rule,
        int maxLength = 200)
    {
        return rule.Add((value, result, path) =>
        {
            if (value == null)
                return;

            if (value.Length > maxLength)
            {
                result.Add(path, $"Too long (max {maxLength}).");
                return;
            }

            if (!SafePattern.IsMatch(value))
                result.Add(path, "Invalid characters for JS-safe string.");
        });
    }
}
