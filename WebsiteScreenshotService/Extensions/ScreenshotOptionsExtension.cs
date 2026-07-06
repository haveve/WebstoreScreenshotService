using Shared.Core.Contracts.ScreeshotModel.Components;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Utils;

using ModelComponents = WebsiteScreenshotService.Model.ScreeshotModel.Components;
using SharedComponents = Shared.Core.Contracts.ScreeshotModel.Components;

namespace WebsiteScreenshotService.Extensions;

public static class ScreenshotOptionsExtension
{
    public static Result<ScreenshotOptionsModel> ToScreenshotOptions(this InputScreenshotModel input, UserContext userContext)
    {
        var validationMessage = Validate(userContext, input);

        if (validationMessage is not null)
            return Result<ScreenshotOptionsModel>.Error(validationMessage);

        var result = new ScreenshotOptionsModel()
        {
            Url = input.Url,
            ScreenshotType = MatchScreenshotType(input.ScreenshotType),
            Clip = input.Clip is not null ? MatchClip(input.Clip) : null,
            Element = input.Element is not null ? MatchClip(input.Element) : null,
            ModalModel = input.ModalModel is not null ? MatchModalModel(input.ModalModel) : null,
            HighlightWord = input.HighlightWord is not null ? MatchHighlightWordModel(input.HighlightWord) : null,
            AdvancedConfiguration = input.AdvancedConfiguration is not null ? MatchAdvancedConfigurationModel(input.AdvancedConfiguration) : null,
            ContentLoadingOptions = MatchModeToContentLoadingOptions(input)
        };

        return Result<ScreenshotOptionsModel>.Success(result);
    }

    public static AdvancedConfigurationModel MatchAdvancedConfigurationModel(ModelComponents.AdvancedConfigurationModel advancedConfigurationModel)
        => new(
            Locale: advancedConfigurationModel.Locale,
            TimezoneId: advancedConfigurationModel.TimezoneId,
            ColorScheme: MatchColorSchemeOption(advancedConfigurationModel.ColorScheme),
            WaitForSelector: advancedConfigurationModel.WaitForSelector,
            BlockResources: MatchColorSchemeOption(advancedConfigurationModel.BlockResources),
            Headers: [.. advancedConfigurationModel.Headers.Select(h => new HeaderModel(h.Name, h.Value))],
            Cookies: [.. advancedConfigurationModel.Cookies.Select(c =>
                new CookieModel(c.Name,
                    c.Value,
                    c.Domain,
                    c.Path,
                    c.Expires,
                    c.Secure,
                    c.HttpOnly,
                    MatchSameSite(c.SameSite)))]
        );

    public static ResourceBlockOptions MatchColorSchemeOption(ModelComponents.ResourceBlockOptions resourceBlockOptions)
    {
        var result = ResourceBlockOptions.None;

        if (resourceBlockOptions.HasFlag(ModelComponents.ResourceBlockOptions.Images))
            result |= ResourceBlockOptions.Images;

        if (resourceBlockOptions.HasFlag(ModelComponents.ResourceBlockOptions.Fonts))
            result |= ResourceBlockOptions.Fonts;

        if (resourceBlockOptions.HasFlag(ModelComponents.ResourceBlockOptions.Media))
            result |= ResourceBlockOptions.Media;

        if (resourceBlockOptions.HasFlag(ModelComponents.ResourceBlockOptions.Scripts))
            result |= ResourceBlockOptions.Scripts;

        if (resourceBlockOptions.HasFlag(ModelComponents.ResourceBlockOptions.Stylesheets))
            result |= ResourceBlockOptions.Stylesheets;

        return result;
    }

    public static ColorSchemeOption MatchColorSchemeOption(ModelComponents.ColorSchemeOption colorSchemeOption)
        => colorSchemeOption switch
        {
            ModelComponents.ColorSchemeOption.NoPreference => ColorSchemeOption.NoPreference,
            ModelComponents.ColorSchemeOption.Light => ColorSchemeOption.Light,
            ModelComponents.ColorSchemeOption.Dark => ColorSchemeOption.Dark,
            _ => throw new InvalidCastException()
        };

    public static SharedComponents.SameSiteMode? MatchSameSite(ModelComponents.SameSiteMode? sameSiteMode)
        => sameSiteMode switch
        {
            ModelComponents.SameSiteMode.Strict => SharedComponents.SameSiteMode.Strict,
            ModelComponents.SameSiteMode.Lax => SharedComponents.SameSiteMode.Lax,
            ModelComponents.SameSiteMode.None => SharedComponents.SameSiteMode.None,
            null => null,
            _ => throw new InvalidCastException()
        };

    public static HighlightWordModel MatchHighlightWordModel(ModelComponents.HighlightWordModel highlightWordModel)
        => new(
            Word: highlightWordModel.Word,
            Color: highlightWordModel.Color
        );

    public static ModalModel MatchModalModel(ModelComponents.ModalModel modalModel)
        => new(
            DismissDialogs: modalModel.DismissDialogs,
            HidePopups: modalModel.HidePopups,
            HideSelectors: modalModel.HideSelectors
        );

    private static ElementModel MatchClip(ModelComponents.ElementModel elementModel)
        => new(
            Selector: elementModel.Selector,
            Clip: new(elementModel.Clip.Width, elementModel.Clip.Height)
        );

    private static ClipModel MatchClip(ModelComponents.ClipModel clipModel)
        => new(
            Width: clipModel.Width,
            Height: clipModel.Height
        );

    private static SharedComponents.ScreenshotType MatchScreenshotType(Model.ScreenshotType screenshotType)
        => screenshotType switch
        {
            Model.ScreenshotType.Jpeg => SharedComponents.ScreenshotType.Jpeg,
            Model.ScreenshotType.Png => SharedComponents.ScreenshotType.Png,
            _ => throw new InvalidCastException()
        };

    private static SharedComponents.ContentLoadingOptions MatchModeToContentLoadingOptions(InputScreenshotModel input)
        => input.Mode switch
        {
            ScreenshotQualityMode.Low => SharedComponents.ContentLoadingOptions.None,
            ScreenshotQualityMode.Medium => input.Element is null ? SharedComponents.ContentLoadingOptions.ScrollToTheEndOfThePage : SharedComponents.ContentLoadingOptions.None,
            ScreenshotQualityMode.High => input.Element is null ? SharedComponents.ContentLoadingOptions.All : SharedComponents.ContentLoadingOptions.WaitForRequestsToComplete,
            _ => throw new InvalidCastException()
        };

    private static string? Validate(UserContext userContext, InputScreenshotModel input)
    {
        var subscriptionType = userContext.SubscriptionPlan.Type;
        var isElementScreenshot = input.Element is not null;
        var hasAdvancedConfigurations = input.AdvancedConfiguration is not null;
        var isHighQualityScreenshot = input.Mode == ScreenshotQualityMode.High;
        var screenshotFullHeight = input.Clip is not null && input.Clip.Height is null;

        if (input.Mode == ScreenshotQualityMode.Low && screenshotFullHeight)
            return "You cannot perform this operation in 'Low' mode. If you think this is an issue, please, contact the administration";

        if (subscriptionType == SubscriptionType.Regular && (isElementScreenshot || hasAdvancedConfigurations || isHighQualityScreenshot))
            return "You cannot perform this operation with 'Regular' subscription. If you think this is an issue, please, contact the administration";

        if (subscriptionType == SubscriptionType.Pro && (isHighQualityScreenshot || hasAdvancedConfigurations))
            return "You cannot perform this operation with 'Pro' subscription. If you think this is an issue, please, contact the administration";

        return null;
    }
}
