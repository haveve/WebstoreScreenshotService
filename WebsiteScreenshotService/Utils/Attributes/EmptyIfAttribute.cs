using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Utils.Attributes;

/// <summary>
/// Validation attribute that enforces a property to be empty if another property has a specified value.
/// </summary>
/// <typeparam name="T">The type of the other property's value.</typeparam>
/// <param name="OtherPropertyName">The name of the other property to check.</param>
/// <param name="OtherValue">The value of the other property that triggers the validation.</param>
public class EmptyIfAttribute<T>(string OtherPropertyName, T OtherValue) : ValidationAttribute
{
    private readonly string _otherPropertyName = OtherPropertyName;

    private readonly T _otherValue = OtherValue;

    /// <summary>
    /// Validates the specified value with respect to the current validation attribute.
    /// </summary>
    /// <param name="value">The value of the property being validated.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A <see cref="ValidationResult"/> that indicates whether the value is valid.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);

        if (otherProperty == null)
            return new ValidationResult($"Unknown property: {_otherPropertyName}");

        var otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

        var currentEmpty = ValidationAttributeHelper.IsUnset(value);
        var otherValuef = EqualOtherValue(otherValue);

        if (otherValuef && !currentEmpty)
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} should be empty if '{_otherValue}' value was specified for {_otherPropertyName}");

        return ValidationResult.Success;
    }

    private bool EqualOtherValue(object? value1)
    {
        if (value1 is null && _otherValue is null)
            return true;

        var equals = value1?.Equals(_otherValue);

        return equals ?? false;
    }
}
