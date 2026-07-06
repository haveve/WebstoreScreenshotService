using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Utils.Attributes;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RequireOneOfAttribute : ValidationAttribute
{
    private readonly string[] _propertyNames;

    /// <summary>
    /// Creates a RequireOneOf attribute that ensures at least one of the specified properties is set (non-null).
    /// </summary>
    /// <param name="propertyNames">The property names to check.</param>
    public RequireOneOfAttribute(params string[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
            throw new ArgumentException("At least one property must be specified.", nameof(propertyNames));

        _propertyNames = propertyNames;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // Null instance is considered valid; other validators can handle null

        // Check if any of the properties have a non-null value
        var hasValue = _propertyNames.Any(name =>
        {
            var property = validationContext.ObjectType.GetProperty(name)
                ?? throw new InvalidOperationException($"Property '{name}' does not exist on type '{validationContext.ObjectType.Name}'.");

            var propertyValue = property.GetValue(value);
            return propertyValue != null;
        });

        if (hasValue)
            return ValidationResult.Success;

        // No property has a value → return validation result
        var errorMessage = ErrorMessage ??
                              $"At least one of the following properties must be provided: {string.Join(", ", _propertyNames)}";

        return new ValidationResult(errorMessage, _propertyNames);
    }
}

