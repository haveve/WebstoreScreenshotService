using ScreenshotWorker.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Model.ScreenshotOptions;

public class ElementModel
{
    [Required]
    [MaxLength(200)]
    [RegularExpressionWithTimeout(@"^[a-zA-Z0-9#\.\[\]\-_\s,:>+~=""'()*]+$", ErrorMessage = "Invalid selector format.")]
    public required string Selector { get; set; }
}
