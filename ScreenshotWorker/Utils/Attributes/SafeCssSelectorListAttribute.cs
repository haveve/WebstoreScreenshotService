using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ScreenshotWorker.Utils.Attributes;

public static partial class CssSelectorValidation
{
    // Matches most common CSS selector characters
    [GeneratedRegex(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$")]
    public static partial Regex SafeSelectorRegex();
}

public class SafeCssSelectorListAttribute : ValidationAttribute
{
    private readonly int _maxCount;
    private readonly int _selectorMaxLength;


    public SafeCssSelectorListAttribute(int maxCount, int selectorMaxLength)
    {
        _maxCount = maxCount;
        _selectorMaxLength = selectorMaxLength;
        ErrorMessage = $"Maximum {_maxCount} selectors allowed, and each must be valid with maximum length of {_selectorMaxLength} characters.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (value is not IEnumerable<string> list)
            return new ValidationResult($"{validationContext.DisplayName} must be a list of strings.");

        if (list.Count() > _maxCount)
            return new ValidationResult(ErrorMessage);

        foreach (var item in list)
        {
            if (string.IsNullOrWhiteSpace(item))
                return new ValidationResult("Selectors cannot be empty.");

            if(item.Length > _selectorMaxLength)
                return new ValidationResult($"Selectors cannot be longer than {_selectorMaxLength}.");

            if (!CssSelectorValidation.SafeSelectorRegex().IsMatch(item))
                return new ValidationResult($"Invalid selector format: {item}");
        }

        return ValidationResult.Success;
    }
}
