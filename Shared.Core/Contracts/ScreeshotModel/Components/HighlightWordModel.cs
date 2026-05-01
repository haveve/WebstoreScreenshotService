using Shared.Core.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Contracts.ScreeshotModel.Components;

public class HighlightWordModel
{
    /// <summary>
    /// Word to highlight. Safe for JS injection, supports Unicode (ru/ua/chinese/etc).
    /// </summary>
    public required string Word { get; set; }

    /// <summary>
    /// Highlight color (hex format only). Safe for JS injection.
    /// </summary>
    public required string Color { get; set; }
}

