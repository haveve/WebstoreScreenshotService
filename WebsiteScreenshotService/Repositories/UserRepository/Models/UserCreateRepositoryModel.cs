using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record UserCreateRepositoryModel(Guid Id, string NickNameHash, string PasswordHash, string Salt, string EncKey, SubscriptionPlan SubscriptionPlan, string EncryptedData);