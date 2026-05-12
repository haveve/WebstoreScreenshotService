using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class UserEntityMapper(IUserContextAccessor userContextAccessor) : EncryptedMapper(userContextAccessor), IUserEntityMapper
{
    public User FromEntity(UserEntity entity)
    {
        var userData = Decrypt(entity.EncryptedData);
        var subscriptionPlan = new SubscriptionPlan(entity.SubscriptionPlan.Type, entity.SubscriptionPlan.ScreenshotLeft);
        return new User(entity.Id, userData.NickName, userData.Email, subscriptionPlan);
    }

    public UserEncryptedData Decrypt(string data)
        => Decrypt<UserEncryptedData>(data, UserEncryptionContextFactory.UserEncEntityData);

    public string Encrypt(UserEncryptedData data)
        => EncryptAsJson(data, UserEncryptionContextFactory.UserEncEntityData);
}
