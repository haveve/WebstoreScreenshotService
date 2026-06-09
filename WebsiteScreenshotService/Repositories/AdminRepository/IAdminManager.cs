using System.Text.Json;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.AdminRepository;

public record AdminCreateManagerModel(
    string NickName,
    string Password,
    string Email);

public interface IAdminManager
{
    Task<Result> CreateAdminAsync(AdminCreateManagerModel model);

    Task<Result<Admin>> GetAdminByNickNameAndPasswordAsync(
        string nickName,
        string password);

    Task<ConditionalResult> DoesAnyAdminExistAsync();

    Task<Result> UpdatePasswordAsync(
        Guid adminId,
        string newPassword);
}