using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MaxCollectionCountAttribute : ValidationAttribute
{
    public int MaxCount { get; }

    public MaxCollectionCountAttribute(int maxCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCount, nameof(maxCount));

        MaxCount = maxCount;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is not IEnumerable enumerable)
            return new ValidationResult(
                $"{validationContext.MemberName} must be a collection.");

        var count = 0;

        foreach (var _ in enumerable)
        {
            count++;

            if (count > MaxCount)
            {
                return new ValidationResult(
                    ErrorMessage ??
                    $"{validationContext.MemberName} cannot contain more than {MaxCount} items.",
                    [ validationContext.MemberName!]);
            }
        }

        return ValidationResult.Success;
    }
}

