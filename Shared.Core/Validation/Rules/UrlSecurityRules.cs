using Shared.Core.Validation.Validators;

namespace Shared.Core.Validation.Rules;

public static class UrlSecurityRules
{
    public static RuleBuilder<T, string> SafeUrl<T>(
        this RuleBuilder<T, string> rule,
        string[]? additionalBlockedHosts = null)
    {
        additionalBlockedHosts ??= [];

        return rule.Add((value, result, path) =>
        {
            if(UrlSecurity.IsIpBlocked(value, additionalBlockedHosts, out string? host))
                result.Add(path, $"url '{value}' was blocked with resolved host: {host}");

            return;
        });
    }
}