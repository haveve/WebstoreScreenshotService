namespace WebsiteScreenshotService.Model.ScreenshotOptions;

public interface IAdvancedConfigurationModel
{
    /// <summary>
    /// Browser locale, e.g., "en-US". Defaults to "en-US".
    /// </summary>
    public string Locale { get; set; }

    /// <summary>
    /// Browser timezone, IANA identifier, e.g., "Europe/Kyiv". Defaults to UTC.
    /// </summary>
    public string TimezoneId { get; set; }

    /// <summary>
    /// Color scheme for screenshot rendering.
    /// </summary>
    public ColorSchemeOption ColorScheme { get; set; }

    public string? WaitForSelector { get; set; }

    public ResourceBlockOptions BlockResources { get; set; }

    public List<IHeaderModel> Headers { get; set; }

    public List<ICookieModel> Cookies { get; set; }
}

public enum ColorSchemeOption
{
    Light,
    Dark,
    NoPreference
}

[Flags]
public enum ResourceBlockOptions
{
    None = 0,
    Images = 1 << 0,
    Fonts = 1 << 1,
    Media = 1 << 2,
    Scripts = 1 << 3,
    Stylesheets = 1 << 4,

    All = Images | Fonts | Media | Scripts | Stylesheets
}