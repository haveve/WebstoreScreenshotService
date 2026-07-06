using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class HashingConfigurations
{
    public const int MinIterations = 10_000;

    [Required]
    [Range(MinIterations, int.MaxValue)]
    public required int Iterations { get; set; }

    [Required]
    [Base64String]
    [Length(32, int.MaxValue)]
    public required string DefaultSalt { get; set; }
}
