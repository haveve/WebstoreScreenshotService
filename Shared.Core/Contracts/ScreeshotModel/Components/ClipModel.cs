namespace Shared.Core.Contracts.ScreeshotModel.Components;

/// <summary>
/// Represents a rectangular region of a webpage to capture in a screenshot.
/// </summary>
public record ClipModel(int Width, int? Height)
{
    public const int MaxWidth = 5000;

    public const int MaxHeight = 7000;
}
