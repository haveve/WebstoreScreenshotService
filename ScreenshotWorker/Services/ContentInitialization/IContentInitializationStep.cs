using OpenQA.Selenium;
using ScreenshotWorker;
using ScreenshotWorker.Model;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public string StepName { get; }

    public Task InitializeScriptsAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions)
       => Task.CompletedTask;

    public Task InitializeAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings Settings);
}
