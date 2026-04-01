using WebsiteScreenshotService.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

public class ElementModel : IElementModel
{
    [Required]
    [MaxLength(200)]
    [RegularExpressionWithTimeout(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", ErrorMessage = "Invalid selector format.")]
    public required string Selector { get; set; }

    [Required]
    public required IElementClip Clip { get; set; }
}

public class ElementClip: IElementClip
{
    public const int MaxWidth = 5000;

    public const int MaxHeight = 7000;

    /// <summary>
    /// Gets or sets the width of the element in pixels.
    /// </summary>
    /// <value>The width in pixels.</value>
    [Required]
    [Range(1, MaxWidth)]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the element in pixels. If is not specified takes the full height of the page.
    /// </summary>
    /// <value>The height in pixels.</value>
    [Required]
    [Range(1, MaxHeight)]
    public int Height { get; set; }
}
