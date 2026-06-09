using Microsoft.AspNetCore.Authorization;

namespace WebsiteScreenshotService.Extensions.ServiceExtensions;

public static class AuthorizationExtension
{
    public static void AddJwtAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>()
            .AddAuthorization(options =>
        {
            options.AddPolicy(Policies.User.FetchScreenshots, p =>
            {
                string[] permissions = [Permissions.User.FullAccess, Permissions.User.Screenshot.ManageScreenshots, Permissions.User.Screenshot.FetchScreenshots];
                p.Requirements.Add(new PermissionRequirements(permissions));
            });

            options.AddPolicy(Policies.User.MakeScreenshots, p =>
            {
                string[] permissions = [Permissions.User.FullAccess, Permissions.User.Screenshot.ManageScreenshots, Permissions.User.Screenshot.MakeScreenshots];
                p.Requirements.Add(new PermissionRequirements(permissions));
            });

            options.AddPolicy(Policies.User.ManageCategories, p =>
            {
                string[] permissions = [Permissions.User.FullAccess, Permissions.User.Category.ManageCategories];
                p.Requirements.Add(new PermissionRequirements(permissions));
            });
        });
    }
}

public record PermissionRequirements(string[] Permissions) : IAuthorizationRequirement;

public class PermissionHandler(IUserContextAccessor userContextAccessor) : AuthorizationHandler<PermissionRequirements>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirements requirement)
    {
        var currentUser = userContextAccessor.TryCurrentUser();

        if (currentUser is null)
            return;

        var currentUserPermissions = currentUser.UserInfo.Permissions;

        var hasPermission = requirement.Permissions.Any(rp => currentUserPermissions.Any(p => p == rp));

        if (hasPermission)
            context.Succeed(requirement);
    }
}
