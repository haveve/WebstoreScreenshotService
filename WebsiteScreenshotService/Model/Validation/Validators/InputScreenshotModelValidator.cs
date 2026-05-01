using Shared.Core.Validation;
using Shared.Core.Validation.Rules;
using WebsiteScreenshotService.Model.ScreeshotModel.Components;

namespace WebsiteScreenshotService.Model.Validation.Validators;

public static class InputLimits
{
    public const int UrlMaxLength = 2048;
    public const int SelectorMaxLength = 200;
    public const int HeaderNameMaxLength = 100;

    public static class ClipModel
    {
        public const int MaxWidth = 5000;
        public const int MaxHeight = 7000;
    }

    public static class ElementClip
    {
        public const int MaxWidth = 5000;

        public const int MaxHeight = 7000;

    }
}

public sealed class InputScreenshotModelValidator : Validator<InputScreenshotModel>
{
    public InputScreenshotModelValidator()
    {
        RuleFor(nameof(InputScreenshotModel.Url), x => x.Url)
            .Required("Url is required.")
            .MaxLen(InputLimits.UrlMaxLength,
                $"Url is too long. Maximum allowed length is {InputLimits.UrlMaxLength} characters.")
            .SafeUrl();

        RuleFor(nameof(InputScreenshotModel.ScreenshotType), x => x.ScreenshotType)
            .Must(v => Enum.IsDefined(v),
                $"Invalid ScreenshotType. Allowed values: {string.Join(", ", Enum.GetNames<ScreenshotType>())}");

        RuleFor(nameof(InputScreenshotModel.ContentLoadingOptions), x => x.ContentLoadingOptions)
            .Must(v => v != ContentLoadingOptions.None,
                "At least one ContentLoadingOption must be selected.")
            .Must(v =>
            {
                var allowed =
                    ContentLoadingOptions.WaitForRequestsToComplete |
                    ContentLoadingOptions.ScrollToTheEndOfThePage;

                return (v & ~allowed) == 0;
            },
            "ContentLoadingOptions contains invalid flags. Allowed: WaitForRequestsToComplete, ScrollToTheEndOfThePage.");

        RuleFor("ClipElementExclusivity", x => x)
            .Must(x => (x.Clip is null) != (x.Element is null),
                "Either Clip or Element must be provided, but not both.");

        Include(nameof(InputScreenshotModel.Clip), x => x.Clip, new ClipValidator());
        Include(nameof(InputScreenshotModel.Element), x => x.Element, new ElementValidator());
        Include(nameof(InputScreenshotModel.ModalModel), x => x.ModalModel, new ModalValidator());
        Include(nameof(InputScreenshotModel.HighlightWord), x => x.HighlightWord, new HighlightValidator());
        Include(nameof(InputScreenshotModel.AdvancedConfiguration), x => x.AdvancedConfiguration, new AdvancedConfigurationValidator());
    }
}

public sealed class ClipValidator : Validator<ClipModel>
{
    public ClipValidator()
    {
        RuleFor(nameof(ClipModel.Width), x => x.Width)
            .Required("Width is required.")
            .Range(1, InputLimits.ClipModel.MaxWidth,
                $"Width must be between 1 and {InputLimits.ClipModel.MaxWidth}.");

        RuleFor(nameof(ClipModel.Height), x => x.Height)
            .Must(v => v is null || (v > 0 && v <= InputLimits.ClipModel.MaxHeight),
                $"Height must be between 1 and {InputLimits.ClipModel.MaxHeight}.");
    }
}

public sealed class ElementValidator : Validator<ElementModel>
{
    public ElementValidator()
    {
        RuleFor(nameof(ElementModel.Selector), x => x.Selector)
            .Required("Selector is required.")
            .SafeCssSelector(string.Empty, InputLimits.SelectorMaxLength);

        Include(nameof(ElementModel.Clip), x => x.Clip, new ElementClipValidator());
    }
}

public sealed class ElementClipValidator : Validator<ElementClip>
{
    public ElementClipValidator()
    {
        RuleFor(nameof(ElementClip.Width), x => x.Width)
            .Required("Width is required.")
            .Range(1, InputLimits.ElementClip.MaxWidth,
                $"Width must be between 1 and {InputLimits.ElementClip.MaxWidth}.");

        RuleFor(nameof(ElementClip.Height), x => x.Height)
            .Required("Height is required.")
            .Range(1, InputLimits.ElementClip.MaxHeight,
                $"Height must be between 1 and {InputLimits.ElementClip.MaxHeight}.");
    }
}

public sealed class ModalValidator : Validator<ModalModel>
{
    public ModalValidator()
    {
        RuleFor(nameof(ModalModel.DismissDialogs), x => x.DismissDialogs)
            .Required("DismissDialogs is required.");

        RuleFor(nameof(ModalModel.HidePopups), x => x.HidePopups)
            .Required("HidePopups is required.");

        RuleFor(nameof(ModalModel.HideSelectors), x => x.HideSelectors)
            .SafeCssSelectorList(
                maxCount: 15,
                selectorMaxLength: InputLimits.SelectorMaxLength,
                msg: "HideSelectors contains invalid CSS selector or exceeds allowed limits.");
    }
}

public sealed class HighlightValidator : Validator<HighlightWordModel>
{
    public HighlightValidator()
    {
        RuleFor(nameof(HighlightWordModel.Word), x => x.Word)
            .Required("Highlight word is required.")
            .SafeJsString(200);

        RuleFor(nameof(HighlightWordModel.Color), x => x.Color)
            .Required("Highlight color is required.")
            .SafeHexColor();
    }
}

public sealed class AdvancedConfigurationValidator : Validator<AdvancedConfigurationModel>
{
    public AdvancedConfigurationValidator()
    {
        RuleFor(nameof(AdvancedConfigurationModel.Locale), x => x.Locale)
            .Required("Locale is required.")
            .Must(RegexPatterns.Locale.IsMatch,
                $"Locale is invalid. Expected format: en-US, fr-FR.");

        RuleFor(nameof(AdvancedConfigurationModel.TimezoneId), x => x.TimezoneId)
            .Required("Timezone is required.")
            .Must(RegexPatterns.Timezone.IsMatch,
                $"Timezone is invalid. Expected format: Europe/Kyiv.");

        RuleFor(nameof(AdvancedConfigurationModel.ColorScheme), x => x.ColorScheme)
            .Must(v => Enum.IsDefined(v),
                $"Invalid ColorScheme.");

        RuleFor(nameof(AdvancedConfigurationModel.WaitForSelector), x => x.WaitForSelector!)
            .SafeCssSelector(string.Empty, InputLimits.SelectorMaxLength);

        RuleFor(nameof(AdvancedConfigurationModel.BlockResources), x => x.BlockResources)
            .Must(v => v >= 0, "BlockResources configuration is invalid.");

        ForEach(nameof(AdvancedConfigurationModel.Headers), x => x.Headers, new HeaderValidator())
            .MaxCollectionLength(20, "Maximum 20 headers allowed.");

        ForEach(nameof(AdvancedConfigurationModel.Cookies), x => x.Cookies, new CookieValidator())
            .MaxCollectionLength(20, "Maximum 20 cookies allowed.");
    }
}

public sealed class HeaderValidator : Validator<HeaderModel>
{
    public HeaderValidator()
    {
        RuleFor(nameof(HeaderModel.Name), x => x.Name)
            .Required("Header name is required.")
            .MaxLen(InputLimits.HeaderNameMaxLength,
                $"Header name must not exceed {InputLimits.HeaderNameMaxLength} characters.")
            .Must(RegexPatterns.Header.IsMatch,
                $"Header name contains invalid characters.");

        RuleFor(nameof(HeaderModel.Value), x => x.Value)
            .Required("Header value is required.")
            .SafeCookieString(1000);
    }
}

public sealed class CookieValidator : Validator<CookieModel>
{
    public CookieValidator()
    {
        RuleFor(nameof(CookieModel.Name), x => x.Name)
            .Required("Cookie name is required.")
            .SafeCookieString(100);

        RuleFor(nameof(CookieModel.Value), x => x.Value)
            .Required("Cookie value is required.")
            .SafeCookieString(1000);

        RuleFor(nameof(CookieModel.Domain), x => x.Domain)
            .Required("Cookie domain is required.")
            .SafeCookieDomain(200);

        RuleFor(nameof(CookieModel.Path), x => x.Path)
            .SafeCookiePath(100);

        RuleFor(nameof(CookieModel.SameSite), x => x.SameSite)
            .Must(v => !v.HasValue || Enum.IsDefined(v.Value),
                $"Invalid SameSite value.");
    }
}