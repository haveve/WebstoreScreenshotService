namespace Shared.Core.Validation.Rules;

public static class BasicRules
{
    public static RuleBuilder<T, string> NotEmpty<T>(
        this RuleBuilder<T, string> rule, string msg)
        => rule.Add((v, r, p) =>
        {
            if (string.IsNullOrWhiteSpace(v))
                r.Add(p, msg);
        });

    public static RuleBuilder<T, TProp> Required<T, TProp>(
        this RuleBuilder<T, TProp> rule,
        string msg,
        bool allowEmptyStrings = false)
    {
        return rule.Add((value, result, path) =>
        {
            if (value is null)
            {
                result.Add(path, msg);
                return;
            }

            if (value is string stringValue && !allowEmptyStrings && string.IsNullOrWhiteSpace(stringValue))
                result.Add(path, msg);
        });
    }

    public static RuleBuilder<T, TCollection> MaxCollectionLength<T, TCollection>(
        this RuleBuilder<T, TCollection> rule,
        int maxCount,
        string message = "Collection exceeds maximum allowed length.") where TCollection : IEnumerable<object>
    {
        return rule.Add((value, result, path) =>
        {
            if (value is null)
                return;

            var count = value.Count();

            if (count > maxCount)
                result.Add(path, message);
        });
    }

    public static RuleBuilder<T, string> MaxLen<T>(
        this RuleBuilder<T, string> rule, int max, string msg)
        => rule.Add((v, r, p) =>
        {
            if (v?.Length > max)
                r.Add(p, msg);
        });

    public static RuleBuilder<T, string> MinLen<T>(
        this RuleBuilder<T, string> rule, int min, string msg)
        => rule.Add((v, r, p) =>
        {
            if (v?.Length < min)
                r.Add(p, msg);
        });

    public static RuleBuilder<T, int> Range<T>(
        this RuleBuilder<T, int> rule, int min, int max, string msg)
        => rule.Add((v, r, p) =>
        {
            if (v < min || v > max)
                r.Add(p, msg);
        });

    public static RuleBuilder<T, TProp> Must<T, TProp>(
        this RuleBuilder<T, TProp> rule,
        Func<TProp, bool> predicate,
        string msg)
        => rule.Add((v, r, p) =>
        {
            if (!predicate(v))
                r.Add(p, msg);
        });
}
