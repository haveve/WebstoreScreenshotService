using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Security;

public class UserCryptographicDataManager(IUserRepository userRepository) : IUserCryptographicDataManager
{
    public async Task<Result<EncryptionInfo>> GetUseCryptographicDataAsync(Guid userId)
    {
        var userInfo = await userRepository.GetUserByIdAsync(userId);

        if (!userInfo.IsSuccess)
            return Result<EncryptionInfo>.Error(userInfo.ErrorMessage!);

        var value = userInfo.Value!;
        return Result<EncryptionInfo>.Success(new(value.Salt, value.EncKey));
    }
}