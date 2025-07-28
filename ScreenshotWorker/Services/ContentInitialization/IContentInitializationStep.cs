using OpenQA.Selenium;
using ScreenshotWorker.Model;
using ScreenshotWorker.Settings.InitializationStep;

namespace ScreenshotWorker.Services.ContentInitialization;

public interface IContentInitializationStep
{
    public string StepName { get; }

    public Task InitializeScriptsAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions)
       => Task.CompletedTask;

    public Task InitializeAsync(WebDriver webDriver, ScreenshotOptionsModel screenshotOptions, ContentInitializationStepSettings Settings);
}
