namespace Shared.Core.Contracts.ScreeshotModel.Components;

public record CookieModel(
    string Name,
    string Value,
    string Domain,
    string Path,
    DateTime? Expires,
    bool Secure,
    bool HttpOnly,
    SameSiteMode? SameSite
    );

public enum SameSiteMode
{
    Strict = 1,
    Lax = 2 ,
    None = 3
}
