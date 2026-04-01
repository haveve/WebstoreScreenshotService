using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Model.ScreenshotOptions;
using WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Extensions;

public static class ScreenshotOptionsExtension
{
    public static Result<ScreenshotOptionsModel> ToScreenshotOptions(this InputScreenshotOptionsModel input, UserContext userContext)
    {
        var validationMessage = Validate(userContext, input);

        if (validationMessage is not null)
            return Result<ScreenshotOptionsModel>.Error(validationMessage);

        var result = new ScreenshotOptionsModel()
        {
            Url = input.Url,
            ScreenshotType = input.ScreenshotType,
            Clip = input.Clip,
            Element = input.Element,
            ModalModel = input.ModalModel,
            HighlightWord = input.HighlightWord,
            AdvancedConfiguration = input.AdvancedConfiguration,
            ContentLoadingOptions = MatchModeToContentLoadingOptions(input)
        };

        return Result<ScreenshotOptionsModel>.Success(result);
    }

    private static ContentLoadingOptions MatchModeToContentLoadingOptions(InputScreenshotOptionsModel input)
        => input.Mode switch
        {
            ScreenshotQualityMode.Low => ContentLoadingOptions.None,
            ScreenshotQualityMode.Medium => input.Element is null ? ContentLoadingOptions.ScrollToTheEndOfThePage : ContentLoadingOptions.None,
            ScreenshotQualityMode.High => input.Element is null ? ContentLoadingOptions.All : ContentLoadingOptions.WaitForRequestsToComplete,
            _ => throw new InvalidCastException()
        };

    private static string? Validate(UserContext userContext, InputScreenshotOptionsModel input)
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

        if (subscriptionType == SubscriptionType.Pro && isHighQualityScreenshot)
            return "You cannot perform this operation with 'Pro' subscription. If you think this is an issue, please, contact the administration";

        return null;
    }
}
