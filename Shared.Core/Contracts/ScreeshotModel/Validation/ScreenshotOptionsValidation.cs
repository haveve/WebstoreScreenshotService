using Shared.Core.Contracts.ScreeshotModel.Components;
using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace Shared.Core.Contracts.ScreeshotModel.Validation;

public sealed class ScreenshotOptionsValidator : Validator<ScreenshotOptionsModel>
{
    public ScreenshotOptionsValidator()
    {
        // ─────────────────────────────
        // URL (DelegateRule + SafeUrl)
        // ─────────────────────────────
        RuleFor(nameof(ScreenshotOptionsModel.Url), x => x.Url)
            .Required("Url is required.")
            .MaxLen(2048, "Url must not exceed 2048 characters.")
            .SafeUrl();

        // ─────────────────────────────
        // ScreenshotType
        // ─────────────────────────────
        RuleFor(nameof(ScreenshotOptionsModel.ScreenshotType), x => x.ScreenshotType)
            .Must(v => Enum.IsDefined(typeof(ScreenshotType), v),
                "Invalid screenshot type.");

        // ─────────────────────────────
        // ContentLoadingOptions
        // ─────────────────────────────
        RuleFor(nameof(ScreenshotOptionsModel.ContentLoadingOptions),
            x => x.ContentLoadingOptions)
            .Must(v => v != ContentLoadingOptions.None,
                "At least one ContentLoadingOptions flag must be set.")
            .Must(v =>
            {
                var allowed =
                    ContentLoadingOptions.WaitForRequestsToComplete |
                    ContentLoadingOptions.ScrollToTheEndOfThePage;

                return (v & ~allowed) == 0;
            }, "Invalid ContentLoadingOptions flags.");

        // ─────────────────────────────
        // Clip OR Element (DelegateRule on root)
        // ─────────────────────────────
        RuleFor("ClipElementExclusivity", x => x)
            .Must(x => x.Clip != null || x.Element != null,
                "You must provide either Clip or Element.")
            .Must(x => !(x.Clip != null && x.Element != null),
                "Only one of Clip or Element can be provided.");

        // ─────────────────────────────
        // Nested rules
        // ─────────────────────────────
        Include(nameof(ScreenshotOptionsModel.Clip),
            x => x.Clip,
            new ClipValidator());

        Include(nameof(ScreenshotOptionsModel.Element),
            x => x.Element,
            new ElementValidator());

        Include(nameof(ScreenshotOptionsModel.ModalModel),
            x => x.ModalModel,
            new ModalValidator());

        Include(nameof(ScreenshotOptionsModel.HighlightWord),
            x => x.HighlightWord,
            new HighlightValidator());

        Include(nameof(ScreenshotOptionsModel.AdvancedConfiguration),
            x => x.AdvancedConfiguration,
            new AdvancedConfigurationValidator());
    }
}

public sealed class ClipValidator : Validator<ClipModel>
{
    public ClipValidator()
    {
        RuleFor(nameof(ClipModel.Width), x => x.Width)
            .Required("Width is required.")
            .Range(1, ClipModel.MaxWidth,
                $"Width must be between 1 and {ClipModel.MaxWidth}.");

        RuleFor(nameof(ClipModel.Height), x => x.Height)
            .Must(v => v is null || (v >= 1 && v <= ClipModel.MaxHeight),
                $"Height must be between 1 and {ClipModel.MaxHeight}.");
    }
}

public sealed class ElementValidator : Validator<ElementModel>
{
    public ElementValidator()
    {
        // ─────────────────────────────
        // Selector (security-sensitive string rule)
        // ─────────────────────────────
        RuleFor(nameof(ElementModel.Selector), x => x.Selector)
            .Required("Selector is required.")
            .SafeCssSelector("", 200);

        // ─────────────────────────────
        // Nested Clip model
        // ─────────────────────────────
        Include(nameof(ElementModel.Clip),
            x => x.Clip,
            new ElementClipValidator());
    }
}

public sealed class ElementClipValidator : Validator<ElementClip>
{
    public ElementClipValidator()
    {
        RuleFor(nameof(ElementClip.Width), x => x.Width)
            .Required("Width is required.")
            .Range(1, ElementClip.MaxWidth,
                $"Width must be between 1 and {ElementClip.MaxWidth}.");

        RuleFor(nameof(ElementClip.Height), x => x.Height)
            .Required("Height is required.")
            .Range(1, ElementClip.MaxHeight,
                $"Height must be between 1 and {ElementClip.MaxHeight}.");
    }
}

public sealed class ModalValidator : Validator<ModalModel>
{
    public ModalValidator()
    {
        // ─────────────────────────────
        // Booleans (explicit rules for clarity)
        // ─────────────────────────────
        RuleFor(nameof(ModalModel.DismissDialogs), x => x.DismissDialogs)
            .Must(_ => true, "DismissDialogs must be provided.");

        RuleFor(nameof(ModalModel.HidePopups), x => x.HidePopups)
            .Must(_ => true, "HidePopups must be provided.");

        // ─────────────────────────────
        // Selector list (reuse your core rule)
        // ─────────────────────────────
        RuleFor(nameof(ModalModel.HideSelectors), x => x.HideSelectors)
            .SafeCssSelectorList(
                maxCount: 15,
                selectorMaxLength: 200,
                msg: "Invalid CSS selector in HideSelectors.");
    }
}

public sealed class HighlightValidator : Validator<HighlightWordModel>
{
    public HighlightValidator()
    {
        // ─────────────────────────────
        // Word (JS-safe string input)
        // ─────────────────────────────
        RuleFor(nameof(HighlightWordModel.Word), x => x.Word)
            .Required("Word is required.")
            .SafeJsString(200);

        // ─────────────────────────────
        // Color (hex validation)
        // ─────────────────────────────
        RuleFor(nameof(HighlightWordModel.Color), x => x.Color)
            .Required("Color is required.")
            .SafeHexColor();
    }
}

public sealed class AdvancedConfigurationValidator : Validator<AdvancedConfigurationModel>
{
    public AdvancedConfigurationValidator()
    {
        // ─────────────────────────────
        // Locale
        // ─────────────────────────────
        RuleFor(nameof(AdvancedConfigurationModel.Locale), x => x.Locale)
            .Required("Locale is required.")
            .Must(v => System.Text.RegularExpressions.Regex.IsMatch(v, @"^[a-z]{2}-[A-Z]{2}$"),
                "Locale must be in format xx-XX (e.g. en-US).");

        // ─────────────────────────────
        // Timezone
        // ─────────────────────────────
        RuleFor(nameof(AdvancedConfigurationModel.TimezoneId), x => x.TimezoneId)
            .Required("Timezone is required.")
            .Must(v => System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Za-z]+\/[A-Za-z_]+$"),
                "Invalid IANA timezone (e.g. Europe/Kyiv).");

        // ─────────────────────────────
        // Enum (always valid, but keeps explicitness)
        // ─────────────────────────────
        RuleFor(nameof(AdvancedConfigurationModel.ColorScheme), x => x.ColorScheme)
            .Must(v => Enum.IsDefined(typeof(ColorSchemeOption), v),
                "Invalid color scheme.");

        // ─────────────────────────────
        // Optional selector (reuse your rule)
        // ─────────────────────────────
        RuleFor(nameof(AdvancedConfigurationModel.WaitForSelector), x => x.WaitForSelector!)
            .Required("")
            .SafeCssSelector("", 200);


        // ─────────────────────────────
        // Flags validation
        // ─────────────────────────────
        RuleFor(nameof(AdvancedConfigurationModel.BlockResources), x => x.BlockResources)
            .Must(v => v >= 0,
                "Invalid resource block configuration.");

        // ─────────────────────────────
        // Collections (security-sensitive)
        // ─────────────────────────────
        ForEach(nameof(AdvancedConfigurationModel.Headers), x => x.Headers, new HeaderValidator())
            .MaxCollectionLength(20, "Maximum 20 headers allowed.");

        ForEach(nameof(AdvancedConfigurationModel.Cookies), x => x.Cookies, new CookieValidator())
             .MaxCollectionLength(15, "Maximum 20 headers allowed.");
    }
}

public sealed class HeaderValidator : Validator<HeaderModel>
{
    public HeaderValidator()
    {
        RuleFor(nameof(HeaderModel.Name), x => x.Name)
            .Required("Header name is required.")
            .MaxLen(100, "Header name is too long.")
            .Must(v => System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Za-z0-9\-]+$"),
                "Header name contains invalid characters.");

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
            .Must(v => !v.HasValue || Enum.IsDefined(typeof(SameSiteMode), v),
                "Invalid SameSite value.");
    }
}