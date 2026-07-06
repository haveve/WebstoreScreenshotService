using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class EncryptionConfigurations
{
    [Required]
    [Base64String]
    [Length(44, 44, ErrorMessage = "Master key should be exactly 44 base64 symbols length")]
    public required string MasterKey { get; set; }
}
