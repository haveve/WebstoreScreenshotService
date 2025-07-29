using Microsoft.Playwright;
using ScreenshotWorker.Model;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public string StepName { get; }

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(true);

    public Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings Settings);
}
