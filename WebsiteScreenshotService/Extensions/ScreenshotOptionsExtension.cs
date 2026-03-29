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
        var subscriptionValidationMessage = ValidateSubscription(userContext, input);

        if (subscriptionValidationMessage is not null)
            return Result<ScreenshotOptionsModel>.Error(subscriptionValidationMessage);

        var result = new ScreenshotOptionsModel()
        {
            Url = input.Url,
            ScreenshotType = input.ScreenshotType,
            Clip = input.Clip,
            Element = input.Element,
            ModalModel = input.ModalModel,
            HighlightWord = input.HighlightWord,
            AdvancedConfiguration = input.AdvancedConfiguration,
            ContentLoadingOptions = MatchModeToContentLoadingOptions(input.Mode)
        };

        return Result<ScreenshotOptionsModel>.Success(result);
    }

    private static ContentLoadingOptions MatchModeToContentLoadingOptions(ScreenshotQualityMode mode)
        => mode switch
        {
            ScreenshotQualityMode.Low => ContentLoadingOptions.None,
            ScreenshotQualityMode.Medium => ContentLoadingOptions.ScrollToTheEndOfThePage,
            ScreenshotQualityMode.High => ContentLoadingOptions.All,
            _ => throw new InvalidCastException()
        };

    private static string? ValidateSubscription(UserContext userContext, InputScreenshotOptionsModel input)
    {
        var subscriptionType = userContext.SubscriptionPlan.Type;
        var isElementScreenshot = input.Element is not null;
        var hasAdvancedConfigurations = input.AdvancedConfiguration is not null;
        var isHighQualityScreenshot = input.Mode == ScreenshotQualityMode.High;

        if (subscriptionType == SubscriptionType.Regular && (isElementScreenshot || hasAdvancedConfigurations || isHighQualityScreenshot))
            return "You cannot perform this operation with 'Regular' subscription. If you think this is an issue, please, contact the administration";

        if(subscriptionType == SubscriptionType.Pro && isHighQualityScreenshot)
            return "You cannot perform this operation with 'Pro' subscription. If you think this is an issue, please, contact the administration";

        return null;
    }
}
