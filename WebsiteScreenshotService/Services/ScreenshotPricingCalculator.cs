using WebsiteScreenshotService.Model.ScreenshotOptions;
using WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

namespace WebsiteScreenshotService.Services;

public static class ScreenshotPricingCalculator
{
    private const decimal BasePoints = 1.0M;

    private const int DefaultWidth = 1920;
    private const int DefaultHeight = 1080;

    public static int CalculatePoints(ScreenshotOptionsModel options)
    {
        var pixels = CalculatePixels(options);

        var sizeMultiplier = pixels / (decimal)(DefaultWidth * DefaultHeight);

        var total = BasePoints * sizeMultiplier;

        // Feature multipliers
        if (IsFullPage(options))
            total *= 1.4M;

        if (options.Element != null)
            total *= 1.5M;

        // Loading complexity
        total *= GetLoadingMultiplier(options.ContentLoadingOptions);

        //advanced settings complexity
        total += GetAdvancedCost(options.AdvancedConfiguration);

        // Additive costs
        if (options.HighlightWord != null)
            total += 0.5M;

        if (options.ModalModel != null)
            total += options.ModalModel.HideSelectors?.Count * 0.1M ?? 0;

        // Safety limits
        total = Math.Max(total, 1);
        total = Math.Min(total, 20);

        return (int)Math.Ceiling(total);
    }

    private static decimal GetAdvancedCost(IAdvancedConfigurationModel? adv)
    {
        if (adv == null)
            return 0;

        var cost = 0.0M;

        // ⏳ Wait for selector (expensive)
        if (!string.IsNullOrWhiteSpace(adv.WaitForSelector))
            cost += 2.0M;

        // 📡 Headers
        if (adv.Headers != null)
            cost += adv.Headers.Count * 0.15M;

        // 🍪 Cookies
        if (adv.Cookies != null)
            cost += adv.Cookies.Count * 0.2M;

        // 🚫 Resource blocking (reduces cost)
        if (adv.BlockResources != ResourceBlockOptions.None)
        {
            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Images))
                cost -= 0.2M;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Fonts))
                cost -= 0.1M;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Media))
                cost -= 0.3M;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Scripts))
                cost -= 0.4M;

            if (adv.BlockResources.HasFlag(ResourceBlockOptions.Stylesheets))
                cost -= 0.1M;
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

    private static decimal GetLoadingMultiplier(ContentLoadingOptions options)
    {
        return options switch
        {
            ContentLoadingOptions.None => 1.0M,
            ContentLoadingOptions.ScrollToTheEndOfThePage => 1.3M,
            ContentLoadingOptions.WaitForRequestsToComplete => 1.5M,
            ContentLoadingOptions.All => 1.8M,
            _ => throw new NotImplementedException()
        };
    }
}