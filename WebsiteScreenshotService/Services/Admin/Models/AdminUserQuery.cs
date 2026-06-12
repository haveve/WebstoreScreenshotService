namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminUserQuery
{
    public int Skip { get; init; }

    public int Take { get; init; }

    public bool? IsDisabled { get; init; }

    public string? NickName { get; init; }

    public int? RefundCount { get; init; }
}
