using ScreenshotWorker.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Model.ScreenshotOptions;

public class HighlightWordModel
{
    /// <summary>
    /// Word to highlight. Safe for JS injection, supports Unicode (ru/ua/chinese/etc).
    /// </summary>
    [Required]
    [SafeJsString(200)]
    public required string Word { get; set; }

    /// <summary>
    /// Highlight color (hex format only). Safe for JS injection.
    /// </summary>
    [Required]
    [SafeHexColor]
    public required string Color { get; set; }
}

