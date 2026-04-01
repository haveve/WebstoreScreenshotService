using WebsiteScreenshotService.Model.ScreenshotOptions;
using WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

namespace WebsiteScreenshotService.Services;

public static class ScreenshotPricingCalculator
{
    private const double BasePoints = 1.0;

    private const int DefaultWidth = 1920;
    private const int DefaultHeight = 1080;

    public static int CalculatePoints(ScreenshotOptionsModel options)
    {
        var pixels = CalculatePixels(options);

        var sizeMultiplier = pixels / (double)(DefaultWidth * DefaultHeight);

        var total = BasePoints * sizeMultiplier;

        // Feature multipliers
        if (IsFullPage(options))
            total *= 1.4;

        if (options.Element != null)
            total *= 1.5;

        // Loading complexity
        total *= GetLoadingMultiplier(options.ContentLoadingOptions);

        //advanced settings complexity
        total += GetAdvancedCost(options.AdvancedConfiguration);

        // Additive costs
        if (options.HighlightWord != null)
            total += 0.5;

        if (options.ModalModel != null)
            total += options.ModalModel.HideSelectors?.Count * 0.2 ?? 0;

        // Safety limits
        total = Math.Max(total, 1);
        total = Math.Min(total, 20);

        return (int)Math.Ceiling(total);
    }

    private static double GetAdvancedCost(IAdvancedConfigurationModel? adv)
    {
        if (adv == null)
            return 0;

        double cost = 0;

        // 🌍 Locale / Timezone (browser context overhead)
        if (!string.IsNullOrWhiteSpace(adv.Locale) && adv.Locale != "en-US")
            cost += 0.2;

        if (!string.IsNullOrWhiteSpace(adv.TimezoneId) && adv.TimezoneId != "UTC")
            cost += 0.2;

        // 🎨 Color scheme
        if (adv.ColorScheme != ColorSchemeOption.NoPreference)
            cost += 0.1;

        // ⏳ Wait for selector (expensive)
        if (!string.IsNullOrWhiteSpace(adv.WaitForSelector))
            cost += 2.0;

        // 📡 Headers
        if (adv.Headers != null)
            cost += adv.Headers.Count * 0.15;

        // 🍪 Cookies
        if (adv.Cookies != null)
            cost += adv.Cookies.Count * 0.2;

        // 🚫 Resource blocking (reduces cost)
        if (adv.BlockResources != ResourceBlockOptions.None)
        {
            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Images))
                cost -= 0.2;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Fonts))
                cost -= 0.1;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Media))
                cost -= 0.3;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Scripts))
                cost -= 0.4;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Stylesheets))
                cost -= 0.1;
        }

        return cost;
    }

    private static long CalculatePixels(ScreenshotOptionsModel options)
    {
        int width = DefaultWidth, height = DefaultHeight;

        if (options.Clip != null)
        {
            width = options.Clip.Width;
            height = options.Clip.Height ?? ClipModel.MaxHeight;
        }

        return (long)width * height;
    }

    private static bool IsFullPage(ScreenshotOptionsModel options)
        => options.Clip != null && options.Clip.Height == null;

    private static double GetLoadingMultiplier(ContentLoadingOptions options)
    {
        return options switch
        {
            ContentLoadingOptions.None => 1.0,
            ContentLoadingOptions.ScrollToTheEndOfThePage => 1.3,
            ContentLoadingOptions.WaitForRequestsToComplete => 1.5,
            ContentLoadingOptions.All => 1.8,
            _ => 1.0
        };
    }
}