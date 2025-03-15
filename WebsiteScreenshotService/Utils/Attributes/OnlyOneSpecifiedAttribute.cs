using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Utils.Attributes;

/// <summary>
/// Validation attribute that ensures only one of the specified properties is set.
/// </summary>
/// <param name="OtherPropertyName">The name of the other property to check.</param>
/// <param name="RequiredIfOtherEmpty">Indicates whether the current property is required if the other property is empty.</param>
public class OnlyOneSpecifiedAttribute(string OtherPropertyName, bool RequiredIfOtherEmpty = false) : ValidationAttribute
{
    private readonly string _otherPropertyName = OtherPropertyName;

    private readonly bool _requiredIfOtherEmpty = RequiredIfOtherEmpty;

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
        var otherEmpty = ValidationAttributeHelper.IsUnset(otherValue);

        if (!otherEmpty && !currentEmpty)
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} and {_otherPropertyName} cannot be specified simultaneously");

        if (currentEmpty && otherEmpty && _requiredIfOtherEmpty)
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} should be specified if {_otherPropertyName} wasn't");

        return ValidationResult.Success;
    }
}
