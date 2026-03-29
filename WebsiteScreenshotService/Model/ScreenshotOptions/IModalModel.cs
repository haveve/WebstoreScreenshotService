namespace WebsiteScreenshotService.Model.ScreenshotOptions;

public interface IModalModel
{
    public bool DismissDialogs { get; set; }

    public bool HidePopups { get; set; }

    /// <summary>
    /// CSS selectors to hide before screenshot.
    /// </summary>
    public List<string> HideSelectors { get; set; }
}
