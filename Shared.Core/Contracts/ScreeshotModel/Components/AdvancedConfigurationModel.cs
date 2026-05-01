namespace Shared.Core.Contracts.ScreeshotModel.Components;

public record AdvancedConfigurationModel(
    string Locale,
    string TimezoneId,
    ColorSchemeOption ColorScheme,
    string? WaitForSelector,
    ResourceBlockOptions BlockResources,
    List<HeaderModel> Headers,
    List<CookieModel> Cookies);

public enum ColorSchemeOption
{
    Light = 1,
    Dark = 2,
    NoPreference = 3
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