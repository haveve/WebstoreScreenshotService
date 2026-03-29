using WebsiteScreenshotService.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

public class ElementModel: IElementModel
{
    [Required]
    [MaxLength(200)]
    [RegularExpressionWithTimeout(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", ErrorMessage = "Invalid selector format.")]
    public required string Selector { get; set; }
}
