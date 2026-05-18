using Microsoft.Playwright;
using Shared.Core.Contracts.ScreeshotModel.Components;
using System.Collections.Concurrent;

namespace ScreenshotWorker.Services;

public sealed class BrowserPool(int maxContexts, int restartAfterJobs) : IAsyncDisposable
{
    private readonly SemaphoreSlim _contextSemaphore = new(maxContexts, maxContexts);
    private readonly SemaphoreSlim _browserRotationSemaphore = new(1, 1);
    private readonly int _restartAfterJobs = restartAfterJobs;

    private readonly ConcurrentDictionary<BrowserWrapper, byte> _browsers = [];
    private IPlaywright? _playwright;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        await AddNewBrowserAsync();
    }

    private async Task<BrowserWrapper> AddNewBrowserAsync()
    {
        var browser = await _playwright!.Chromium.LaunchAsync(new()
        {
            Headless = true, // keep headless for automation
            ChromiumSandbox = true,
            Args = [
                "--disable-blink-features=AutomationControlled", // reduces basic bot detection
                "--disable-background-networking", // reduces unnecessary network calls
                "--disable-sync",                 // avoids syncing any account info
                "--disable-translate",            // disables translation requests
                "--disable-extensions",           // prevents extensions loading (safer)
                "--disable-component-update",     // blocks auto-updates of components
                "--disable-service-worker",       // disabled service workers
                "--disable-default-apps"          // disables default Chrome apps
            ],
            HandleSIGHUP = true,
            HandleSIGINT = true,
            HandleSIGTERM = true,
            Timeout = 30000 // 30s launch timeout
        });

        var wrapper = new BrowserWrapper(browser);
        _browsers[wrapper] = 1;

        return wrapper;
    }

    public async Task<PooledContext> AcquireContextAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        await _contextSemaphore.WaitAsync();

        var target = await GetBrowserAndRotateIfNeeded();

        var browserContextOptions = CreateBrowserContextOptions(screenshotOptionsModel);
        var context = await target.Browser.NewContextAsync(browserContextOptions);

        return new PooledContext(context, () => ReleaseContext(target));
    }

    private async Task<BrowserWrapper> GetBrowserAndRotateIfNeeded()
    {
        var target = _browsers.Keys.FirstOrDefault(b => !b.Retiring);

        if (target is not null)
        {
            Interlocked.Increment(ref target.JobCounter);

            if (target.JobCounter <= _restartAfterJobs)
                return target;
        }

        try
        {
            await _browserRotationSemaphore.WaitAsync();

            target = _browsers.Keys.FirstOrDefault(b => !b.Retiring) ?? await AddNewBrowserAsync();
            Interlocked.Increment(ref target.JobCounter);

            // retire browser if job limit reached
            if (target.JobCounter > _restartAfterJobs)
            {
                target.Retiring = true;
                target = await AddNewBrowserAsync();
            }

            foreach(var retiredBrowser in _browsers.Keys.Where(b => b.Retiring && target.ActiveContexts == 0))
                ReleaseContext(retiredBrowser);

            return target;
        }
        finally
        {
            _browserRotationSemaphore.Release();
        }
    }

    private static BrowserNewContextOptions CreateBrowserContextOptions(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var advancedConfig = screenshotOptionsModel.AdvancedConfiguration;
        var config = screenshotOptionsModel;

        var browserContextOptions = new BrowserNewContextOptions
        {
            JavaScriptEnabled = true,
            BypassCSP = false,
            Locale = advancedConfig?.Locale ?? "en-US",
            HasTouch = false,
            DeviceScaleFactor = 1,
            IgnoreHTTPSErrors = false,
            ViewportSize = new ViewportSize
            {
                Width = config.Element?.Clip.Width ?? config.Clip?.Width ?? 1280,
                Height = config.Element?.Clip.Height ?? config.Clip?.Height ?? 800
            },
            ColorScheme = advancedConfig?.ColorScheme switch
            {
                ColorSchemeOption.Light => ColorScheme.Light,
                ColorSchemeOption.Dark => ColorScheme.Dark,
                _ => ColorScheme.NoPreference
            },
            AcceptDownloads = false,
            TimezoneId = advancedConfig?.TimezoneId ?? "UTC",
            Permissions = [],
        };

        return browserContextOptions;
    }

    private void ReleaseContext(BrowserWrapper wrapper)
    {
        Interlocked.Decrement(ref wrapper.ActiveContexts);
        _contextSemaphore.Release();

        if (!wrapper.Retiring || wrapper.ActiveContexts > 0)
            return;

        _ = Task.Run(async () =>
        {
            try
            {
                await wrapper.Browser.CloseAsync();
            }
            catch { }

            _browsers.Remove(wrapper, out byte _);
        });

    }

    public async ValueTask DisposeAsync()
    {
        var wrappers = _browsers.Keys.ToArray();
        _browsers.Clear();

        foreach (var w in wrappers)
            try { await w.Browser.CloseAsync(); } catch { }

        _playwright?.Dispose();
        _contextSemaphore.Dispose();
    }

    private sealed class BrowserWrapper(IBrowser browser)
    {
        public IBrowser Browser { get; } = browser;
        public int JobCounter;
        public int ActiveContexts;
        public bool Retiring;
    }
}