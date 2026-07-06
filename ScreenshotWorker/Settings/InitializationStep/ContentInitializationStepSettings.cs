using ScreenshotWorker.Services.ContentInitialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Settings.InitializationStep;

[JsonDerivedType(typeof(ScrollInitializationStepSettings), typeDiscriminator: ContentInitializationStepsNames.Scroll)]
[JsonDerivedType(typeof(WaitForElementToAppearInitializationStepSettings), typeDiscriminator: ContentInitializationStepsNames.WaitForElementToAppear)]
[JsonDerivedType(typeof(RequestsToCompleteInitializationStepSettings), typeDiscriminator: ContentInitializationStepsNames.RequestsToComplete)]
[JsonDerivedType(typeof(WaitForSelectorInitializationStepSettings), typeDiscriminator: ContentInitializationStepsNames.WaitForSelector)]
public class ContentInitializationStepSettings
{
    [Required]
    [Range(0, 720)]
    public float ExecutionTimeoutInSeconds { get; set; }
}

public sealed class RequestsToCompleteInitializationStepSettings
    : ContentInitializationStepSettings
{
}

public sealed class WaitForSelectorInitializationStepSettings
    : ContentInitializationStepSettings
{
}

public sealed class WaitForElementToAppearInitializationStepSettings
    : ContentInitializationStepSettings
{
}