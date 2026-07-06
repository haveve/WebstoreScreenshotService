using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record UpdateApiTokenUsageModel(TokenLocation UsedLocation, string TokenHash);