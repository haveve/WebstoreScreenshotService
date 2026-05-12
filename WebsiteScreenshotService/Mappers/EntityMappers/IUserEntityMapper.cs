using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers;

public interface IUserEntityMapper : IEntityMapper<UserEntity, User>
{
    public UserEncryptedData Decrypt(string data);

    public string Encrypt(UserEncryptedData data);
};
