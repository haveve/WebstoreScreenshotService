using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class RefreshTokenEntityMapper : IRefreshTokenEntityMapper
{
    public RefreshToken FromEntity(RefreshTokenEntity entity)
    {
        var tokenMetadata = new Entities.RefreshTokenMetadata(
            entity.TokenMetadata.Issued,
            MatchTokenLocation(entity.TokenMetadata.IssuedLocation)!,
            entity.TokenMetadata.Revoked,
            MatchTokenLocation(entity.TokenMetadata.RevokeLocation));
        
        return new RefreshToken(
            entity.UserId, 
            entity.TokenHash,
            entity.FamilyId,
            entity.Expires,
            entity.ReplacedByTokenId,
            entity.RevokedReason,
            tokenMetadata);
    }

    private static Entities.TokenLocation? MatchTokenLocation(
        Repositories._EF.DbEntities.Auth.TokenLocation? tokenLocation)
        => tokenLocation is not null
            ? new(tokenLocation.CountryCode, tokenLocation.Country, tokenLocation.City)
            : null;
}
