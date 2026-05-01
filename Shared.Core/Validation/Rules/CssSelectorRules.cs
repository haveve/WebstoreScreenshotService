using System.Text.RegularExpressions;

namespace Shared.Core.Validation.Rules;

public static partial class CssSelectorValidation
{
    // Matches most common CSS selector characters
    [GeneratedRegex(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", RegexOptions.None, matchTimeoutMilliseconds: 100)]
    public static partial Regex SafeSelectorRegex();
}

public static class CssSelectorRules
{
    public static RuleBuilder<T, TProp> SafeCssSelectorList<T, TProp>(
        this RuleBuilder<T, TProp> rule,
        int maxCount,
        int selectorMaxLength,
        string msg = "Invalid selector format.") where TProp: IEnumerable<string>
    {
        return rule.Add((list, result, path) =>
        {
            if (list is null)
                return;

            var items = list as IList<string> ?? list.ToList();

            if (items.Count > maxCount)
            {
                result.Add(path, $"Maximum {maxCount} selectors allowed.");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ValidateSelector(
                    items[i],
                    result,
                    $"{path}[{i}]",
                    selectorMaxLength,
                    msg);
            }
        });
    }

    public static RuleBuilder<T, string> SafeCssSelector<T>(
        this RuleBuilder<T, string> rule,
        string msg,
        int selectorMaxLength)
    {
        return rule.Add((value, result, path) =>
        {
            ValidateSelector(
                value,
                result,
                path,
                selectorMaxLength,
                msg);
        });
    }

    public static void ValidateSelector(
        string? value,
        ValidationResult result,
        string path,
        int selectorMaxLength,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.Add(path, "Selectors cannot be empty.");
            return;
        }

        if (value.Length > selectorMaxLength)
        {
            result.Add(path, $"Selector must not exceed {selectorMaxLength} characters.");
            return;
        }

        if (!CssSelectorValidation.SafeSelectorRegex().IsMatch(value))
        {
            result.Add(path, errorMessage);
        }
    }
}
