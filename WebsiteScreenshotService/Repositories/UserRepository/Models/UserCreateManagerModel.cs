using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record UserCreateManagerModel(string NickName, string Email, string Password, SubscriptionPlan SubscriptionPlan);