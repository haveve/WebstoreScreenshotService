using Microsoft.Playwright;
using ScreenshotWorker.Settings.InitializationStep;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public string StepName { get; }

    public ValueTask<bool> IsAvailable(IPage page, ScreenshotOptionsModel screenshotOptions)
        => ValueTask.FromResult(true);

    public Task InitializeAsync(IPage page, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings Settings);
}
