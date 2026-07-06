using Microsoft.Playwright;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationManager
{
    public Task InitializeContentAsync(IPage page, ScreenshotOptionsModel screenshotOptions);
}
