namespace Shared.Core.Contracts.ScreeshotModel.Components;

public record ElementModel(string Selector, ElementClip Clip);

public record ElementClip(int Width, int Height);