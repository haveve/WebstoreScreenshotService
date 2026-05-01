using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using ScreenshotWorker.Services.ContentInitialization;
using ScreenshotWorker.Settings;
using Shared.Core.Contracts.ScreeshotModel.Components;
using System.Collections.Immutable;

namespace ScreenshotWorker.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class BrowserService(IContentInitializationManager contentInitializationManager, IOptions<BrowserServiceSettings> _browserServiceSettings, BrowserPool browserPool) : IBrowserService
{
    private readonly IContentInitializationManager _contentInitializationManager = contentInitializationManager;
    private readonly BrowserServiceSettings _browserServiceSettings = _browserServiceSettings.Value;
    private readonly BrowserPool _browserPool = browserPool;

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<byte[]> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        PooledContext? pooledContext = null;

        try
        {
            pooledContext = await CreateBrowserContextAsync(screenshotOptionsModel);
            var context = pooledContext.Context;
            var page = await context.NewPageAsync();

            await page.GotoAsync(screenshotOptionsModel.Url, new()
            {
                WaitUntil = WaitUntilState.Commit,
            });

            await ConfigureResourceBlockingAsync(page, screenshotOptionsModel);

            await _contentInitializationManager.InitializeContentAsync(page, screenshotOptionsModel);

            await HidePopupsAsync(page, screenshotOptionsModel);
            await ApplyHighlightWordAsync(page, screenshotOptionsModel);

            if (screenshotOptionsModel.Element is not null)
            {
                var element = await page.QuerySelectorAsync(screenshotOptionsModel.Element.Selector)
                    ?? throw new InvalidOperationException("Element not found.");

                return await element.ScreenshotAsync(FormatElementScreenshotOptions(screenshotOptionsModel));
            }

            if (screenshotOptionsModel.Clip is not null)
                return await page.ScreenshotAsync(FormatScreenshotOptions(screenshotOptionsModel));

            //never should be throws and either Clip or Element is required during parsing
            throw new InvalidOperationException("You must provide either Clip or Element.");
        }
        finally
        {
            if (pooledContext != null)
                await pooledContext.DisposeAsync();
        }
    }

    private static readonly ImmutableList<string> defaultSelectors =
            [
            // Generic UI patterns
            "[class*='modal']",
            "[class*='popup']",
            "[class*='overlay']",
            "[class*='dialog']",

            // Cookie / GDPR
            "[class*='cookie']",
            "[class*='consent']",
            "[class*='gdpr']",
            "[class*='privacy']",

            // Marketing / interruptions
            "[class*='banner']",
            "[class*='subscribe']",
            "[class*='newsletter']",

            // Backdrops / blockers
            "[class*='backdrop']",
            "[class*='lightbox']"
            ];

    private static async ValueTask HidePopupsAsync(IPage page, ScreenshotOptionsModel screenshotOptionsModel)
    {
        if (screenshotOptionsModel.ModalModel is null)
            return;

        if (screenshotOptionsModel.ModalModel.DismissDialogs)
            page.Dialog += async (_, dialog) => await dialog.DismissAsync();

        if (!screenshotOptionsModel.ModalModel.HidePopups)
            return;

        var selectors = defaultSelectors
            .Concat(screenshotOptionsModel.ModalModel.HideSelectors)
            .Distinct()
            .ToArray();

        await page.EvaluateAsync(@"(selectors) => {
                selectors.forEach(sel => {
                    const elements = document.querySelectorAll(sel);
                    elements.forEach(el => el.style.display = 'none');
                });
            }", selectors);
    }

    private static PageScreenshotOptions FormatScreenshotOptions(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var options = new PageScreenshotOptions()
        {
            FullPage = screenshotOptionsModel.Clip?.Height is null,
            Type = MatchScreenshotType(screenshotOptionsModel)
        };

        return options;
    }

    private static ElementHandleScreenshotOptions FormatElementScreenshotOptions(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var options = new ElementHandleScreenshotOptions()
        {
            Type = MatchScreenshotType(screenshotOptionsModel)
        };

        return options;
    }

    private static Microsoft.Playwright.ScreenshotType MatchScreenshotType(ScreenshotOptionsModel screenshotOptionsModel)
    {
        return screenshotOptionsModel.ScreenshotType switch
        {
            Shared.Core.Contracts.ScreeshotModel.Components.ScreenshotType.Png => Microsoft.Playwright.ScreenshotType.Png,
            Shared.Core.Contracts.ScreeshotModel.Components.ScreenshotType.Jpeg => Microsoft.Playwright.ScreenshotType.Jpeg,
            _ => throw new NotImplementedException("Invalid image type")
        };
    }

    private async Task<PooledContext> CreateBrowserContextAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var pooledContext = await _browserPool.AcquireContextAsync(screenshotOptionsModel);
        var context = pooledContext.Context;

        await ApplyHeadersCookiesAndUserAgentAsync(context, screenshotOptionsModel);

        context.SetDefaultNavigationTimeout(_browserServiceSettings.NavigationTimeout * 1000);
        context.SetDefaultTimeout(_browserServiceSettings.DefaultTimeout * 1000);

        return pooledContext;
    }

    private static async ValueTask ApplyHeadersCookiesAndUserAgentAsync(IBrowserContext context, ScreenshotOptionsModel options)
    {
        var advancedOptions = options.AdvancedConfiguration;

        if (advancedOptions is null)
            return;

        if (advancedOptions.Headers.Count > 0)
        {
            var safeHeaders = advancedOptions.Headers
                .Where(h => !Constants.BlockedHeaders.Contains(h.Name, StringComparer.OrdinalIgnoreCase))
                .DistinctBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(h => h.Name, h => h.Value);

            const string AccepLanguageHeader = "Accept-Language";

            if (!safeHeaders.ContainsKey(AccepLanguageHeader))
                safeHeaders[AccepLanguageHeader] = "en-US,en;q=0.9";

            await context.SetExtraHTTPHeadersAsync(safeHeaders);
        }

        if (advancedOptions.Cookies.Count > 0)
        {
            var cookies = advancedOptions.Cookies.Select(c => new Cookie
            {
                Name = c.Name,
                Value = c.Value,
                Domain = c.Domain,
                Path = c.Path ?? "/",
                Expires = c.Expires.HasValue ? new DateTimeOffset(c.Expires.Value).ToUnixTimeSeconds() : null,
                Secure = c.Secure,
                HttpOnly = c.HttpOnly,
                SameSite = c.SameSite switch
                {
                    SameSiteMode.Strict => SameSiteAttribute.Strict,
                    SameSiteMode.None => SameSiteAttribute.None,
                    _ => SameSiteAttribute.Lax
                },
            });

            await context.AddCookiesAsync(cookies);
        }
    }

    private static async ValueTask ApplyHighlightWordAsync(IPage page, ScreenshotOptionsModel options)
    {
        if (options.HighlightWord is null)
            return;

        await page.EvaluateAsync(@"(model) => {
            const walker = document.createTreeWalker(
                document.body,
                NodeFilter.SHOW_TEXT
            );

            const nodes = [];

            while (walker.nextNode()) {
                if (walker.currentNode.nodeValue.includes(model.word))
                    nodes.push(walker.currentNode);
            }

            nodes.forEach(node => {
                const span = document.createElement('span');
                span.innerHTML = node.nodeValue.replace(
                    new RegExp(model.word, 'gi'),
                    m => `<mark style='background:${model.color}'>${m}</mark>`
                );
                node.replaceWith(span);
            });
        }", options.HighlightWord);
    }

    private static async Task ConfigureResourceBlockingAsync(IPage page, ScreenshotOptionsModel options)
    {
        var blockOptions = options.AdvancedConfiguration?.BlockResources;

        if (blockOptions is null || blockOptions == ResourceBlockOptions.None)
            return;

        await page.RouteAsync("**/*", async route =>
        {
            var resourceType = route.Request.ResourceType;

            var shouldBlock =
                (blockOptions.Value.HasFlag(ResourceBlockOptions.Images) && resourceType == "image") ||
                (blockOptions.Value.HasFlag(ResourceBlockOptions.Fonts) && resourceType == "font") ||
                (blockOptions.Value.HasFlag(ResourceBlockOptions.Media) && resourceType == "media") ||
                (blockOptions.Value.HasFlag(ResourceBlockOptions.Scripts) && resourceType == "script") ||
                (blockOptions.Value.HasFlag(ResourceBlockOptions.Stylesheets) && resourceType == "stylesheet");

            if (shouldBlock)
            {
                await route.AbortAsync();
                return;
            }

            await route.ContinueAsync();
        });
    }
}
