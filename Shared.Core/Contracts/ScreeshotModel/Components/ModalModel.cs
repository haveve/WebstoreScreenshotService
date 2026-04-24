using System.ComponentModel.DataAnnotations;
using Shared.Core.Validation.Attributes;

namespace Shared.Core.Contracts.ScreeshotModel.Components;

public class ModalModel
{
    [Required]
    public bool DismissDialogs { get; set; }

    [Required]
    public bool HidePopups { get; set; }

    /// <summary>
    /// CSS selectors to hide before screenshot.
    /// </summary>
    [SafeCssSelectorList(maxCount: 15, selectorMaxLength: 200)]
    public List<string> HideSelectors { get; set; } = [];
}
