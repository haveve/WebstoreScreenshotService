using Shared.Core.Contracts.ScreeshotModel.Components;

namespace WebsiteScreenshotService.Services;

public interface IScreenshotCalculator
{
    int CalculatePoints(ScreenshotOptionsModel options);
}
