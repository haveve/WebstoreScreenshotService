namespace Shared.Core.Validation.Rules;

public static class LogicalRules
{
    public static void RequireExactlyOne<T>(
        this Validator<T> validator,
        string message,
        params Func<T, object?>[] selectors)
    {
        if (selectors == null || selectors.Length == 0)
            throw new ArgumentException("At least one selector is required.");

        validator.Rule((instance, result, path) =>
        {
            var count = selectors.Count(selector => selector(instance) is not null);

            if (count != 1)
            {
                result.Add(
                    path,
                    message ?? "Exactly one value must be provided.");
            }
        });
    }
}
