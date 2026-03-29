namespace WebsiteScreenshotService.Model.ScreenshotOptions;

public interface IHeaderModel
{
    /// <summary>
    /// HTTP header name.
    /// Example: Accept-Language
    /// Must contain only letters, digits, and dash.
    /// Prevents header injection attacks.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// HTTP header value.
    /// Prevents CR/LF injection.
    /// Max length 4000.
    /// </summary>
    public string Value { get; set; }
}
