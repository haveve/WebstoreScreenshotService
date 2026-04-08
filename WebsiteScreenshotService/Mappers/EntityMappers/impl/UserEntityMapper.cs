using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;

namespace WebsiteScreenshotService.Mappers.EntityMappers.impl;

public class UserEntityMapper(IUserContextAccessor userContextAccessor) : EncryptedMapper(userContextAccessor), IUserEntityMapper
{
    public User FromEntity(UserEntity entity)
    {
        var userData = Decrypt<EncryptedData>(entity.EncryptedData);
        var subscriptionPlan = new SubscriptionPlan(entity.SubscriptionPlan.Type, entity.SubscriptionPlan.ScreenshotLeft); ;
        return new User(entity.Id, userData.Name, userData.Email, subscriptionPlan);
    }

    public EncryptedData Decrypt(string data)
        => Decrypt<EncryptedData>(data);

    public string Encrypt(EncryptedData data)
        => Encrypt(data);
}
