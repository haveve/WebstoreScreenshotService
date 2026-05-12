using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService;

public record UserContext(UserInfo UserInfo, SubscriptionPlan SubscriptionPlan);

public class UserRoles
{
    public const string User = "User";
}

public enum UserRole
{
    User = 1,
}

public class UserSpecificServices
{
    public required IUserEncryptionService EncryptionService { get; init; }

    public required IUserHashingService HashingService { get; init; }
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

        public static class Category
        {
            public const string ManageCategories = "user.categories.manage";
        }

        public const string FullAccess = "user.fullaccess";
    }
}

public static class Policies
{
    public static class User
    {
        public const string FetchScreenshots = "FetchScreenshots";

        public const string MakeScreenshots = "MakeScreenshots";

        public const string ManageCategories = "ManageCategories";
    }
}