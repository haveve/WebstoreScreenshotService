using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record UserCreateModel(Guid Id, string Name, string Email, string Password, SubscriptionPlan SubscriptionPlan);