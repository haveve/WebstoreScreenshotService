using System.Collections.Immutable;

namespace ScreenshotWorker.Services.ContentInitialization;

public static class ContentInitializationStepsNames
{
    public const string Scroll = "Scroll";

    public const string RequestsToComplete = "RequestsToComplete";

    public static IImmutableList<string> Steps { get; } = ImmutableList.Create(Scroll, RequestsToComplete);
}
