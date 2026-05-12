using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class ApiTokenEntityMapper(IUserContextAccessor userContextAccessor) : EncryptedMapper(userContextAccessor), IApiTokenEntityMapper
{
    public ApiToken FromEntity(ApiTokenEntity entity)
    {
        var tokenData = Decrypt(entity.EncryptedData);
        var tokenMetadata = new Entities.ApiTokenMetadata(
            entity.TokenMetadata.Issued,
            MatchTokenLocation(entity.TokenMetadata.IssuedLocation)!,
            entity.TokenMetadata.LastUsed,
            MatchTokenLocation(entity.TokenMetadata.LastUsedLocation),
            entity.TokenMetadata.Revoked,
            MatchTokenLocation(entity.TokenMetadata.RevokeLocation));
        
        return new ApiToken(
            entity.UserId, 
            entity.Name, 
            entity.TokenHash, 
            entity.Expires, 
            entity.RevokedReason, 
            tokenData.AllowedIps, 
            tokenData.Scopes, 
            tokenMetadata);
    }

    private static Entities.TokenLocation? MatchTokenLocation(
        Repositories._EF.DbEntities.Auth.TokenLocation? tokenLocation)
        => tokenLocation is not null
            ? new(tokenLocation.CountryCode, tokenLocation.Country, tokenLocation.City)
            : null;

    public ApiTokenEncryptedData Decrypt(string data)
        => Decrypt<ApiTokenEncryptedData>(data, UserEncryptionContextFactory.ApiTokenEncEntityData);

    public string Encrypt(ApiTokenEncryptedData data)
        => EncryptAsJson(data, UserEncryptionContextFactory.ApiTokenEncEntityData);
}
