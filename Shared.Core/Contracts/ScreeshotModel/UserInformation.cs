using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Contracts.ScreeshotModel;

public class UserInformation
{
    [Required]
    public required Guid UserId { get; set; }
}
