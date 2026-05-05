using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService;

public record UserContext(UserInfo UserInfo, SubscriptionPlan SubscriptionPlan);

public enum UserRole
{
    User = 1,
}

public class UserSpecificServices
{
    public IUserEncryptionService EncryptionService { get; init; } = null!; 
}

public record UserInfo(Guid Id, UserRole Role, string[] Permissions);

public static class Permissions
{
    public static class User
    {
        public static class Screenshot
        {
            public const string FetchScreenshots = "user.screenshots.fetch";

            public const string MakeScreenshots = "user.screenshots.make";

            public const string ManageScreenshots = "user.screenshots.manage";
        }

        public const string FullAccess = "user.fullaccess";
    }
}