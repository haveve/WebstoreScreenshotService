namespace Shared.Core.Validation.Rules;

public static class CollectionRules
{
    public static RuleBuilder<T, IEnumerable<TItem>> MaxCount<T, TItem>(
        this RuleBuilder<T, IEnumerable<TItem>> rule,
        int maxCount,
        string message)
    {
        if (maxCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        return rule.Add((value, result, path) =>
        {
            if (value == null)
                return;

            int count = 0;

            foreach (var _ in value)
            {
                count++;

                if (count > maxCount)
                {
                    result.Add(
                        path,
                        message ?? $"{path} cannot contain more than {maxCount} items.");
                    return;
                }
            }
        });
    }
}
