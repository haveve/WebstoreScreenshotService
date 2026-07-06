using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.AdminRepository;

public record AdminCreateRepositoryModel(
    Guid Id,
    string NickNameHash,
    string PasswordHash,
    string Salt,
    string EncKey,
    string EncryptedData);


public interface IAdminRepository
{
    Task<ConditionalResult> DoesAnyAdminExistAsync();

    Task<Result<AdminEntity>> CreateAdminAsync(
        AdminCreateRepositoryModel model);

    Task<Result<AdminEntity>> GetAdminByCredentialsAsync(
        string nickNameHash,
        string passwordHash);

    Task<Result<AdminEntity>> GetAdminByNickNameAsync(
        string nickNameHash);

    Task<Result<AdminEntity>> GetAdminByIdAsync(Guid id);

    Task<Result> UpdatePasswordAsync(
        Guid id,
        string passwordHash);
}
