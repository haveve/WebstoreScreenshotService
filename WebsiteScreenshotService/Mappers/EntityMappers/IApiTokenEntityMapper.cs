using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

namespace WebsiteScreenshotService.Mappers.EntityMappers;

public interface IApiTokenEntityMapper: IEntityMapper<ApiTokenEntity, ApiToken>
{
    public ApiTokenEncryptedData Decrypt(string data);

    public string Encrypt(ApiTokenEncryptedData data);
}
