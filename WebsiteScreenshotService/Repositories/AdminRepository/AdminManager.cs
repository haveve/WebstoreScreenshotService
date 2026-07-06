using Microsoft.Extensions.Options;
using System.Text.Json;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Services.Security;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.AdminRepository;

public class AdminManager(
    IAdminRepository adminRepository,
    IHashingService hashingService,
    IKeyService keyService,
    IEncryptionService encryptionService,
    IOptions<EncryptionConfigurations> encryptionConfig
) : IAdminManager
{
    private readonly IAdminRepository _adminRepository = adminRepository;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly EncryptionConfigurations _config = encryptionConfig.Value;

    public async Task<Result> CreateAdminAsync(AdminCreateManagerModel model)
    {
        var nickNameHash = _hashingService.Hash(model.NickName);

        var exists = await _adminRepository.DoesAnyAdminExistAsync();

        if (exists.IsSuccess)
            return Result.Error("Admin already exists");

        var salt = _keyService.GenerateBase64Key();
        var encKey = _keyService.GenerateBase64Key();
        var adminId = Guid.CreateVersion7();

        var encryptedData = new AdminEncryptedData(
            model.NickName,
            model.Email,
            TwoFactorModel: null);

        var encryptedEncKey = _encryptionService.Encrypt(
            encKey,
            _keyService.FromBase64(_config.MasterKey),
            new(adminId.ToString(), "admin-enc-key"));

        var encryptedAdminData = _encryptionService.Encrypt(
            JsonSerializer.Serialize(encryptedData),
            _keyService.FromBase64(encKey),
            new(adminId.ToString(), "admin-data"));

        var entity = new AdminCreateRepositoryModel(
            Id: adminId,
            NickNameHash: nickNameHash,
            PasswordHash: _hashingService.Hash(model.Password, salt),
            Salt: salt,
            EncKey: encryptedEncKey,
            EncryptedData: encryptedAdminData
        );

        var result = await _adminRepository.CreateAdminAsync(entity);

        return result.IsSuccess
            ? Result.Success
            : Result.Error(result.ErrorMessage!);
    }

    public async Task<Result<Admin>> GetAdminByNickNameAndPasswordAsync(
        string nickName,
        string password)
    {
        var nickNameHash = _hashingService.Hash(nickName);

        var admin = await _adminRepository
            .GetAdminByNickNameAsync(nickNameHash);

        if (!admin.IsSuccess)
            return Result<Admin>.Error("Invalid credentials");

        var entity = admin.Value!;

        if (!_hashingService.Verify(password, entity.Salt, entity.PasswordHash))
            return Result<Admin>.Error("Invalid credentials");

        var encKey = _encryptionService.Decrypt(
            entity.EncKey,
            _keyService.FromBase64(_config.MasterKey),
            new(entity.Id.ToString(), "admin-enc-key"));

        var encDataBase64 = _encryptionService.Decrypt(
            entity.EncryptedData,
            _keyService.FromBase64(encKey),
            new(entity.Id.ToString(), "admin-data"));

        var adminId = admin.Value!.Id;

        var encData = JsonSerializer.Deserialize<AdminEncryptedData>(encDataBase64)
            ?? throw new InvalidCastException($"Cannot parse encrypted admin data for {adminId}");

        var adminData = new Admin(adminId, encData.Email, encData.NickName);

        return Result<Admin>.Success(adminData);
    }

    public async Task<ConditionalResult> DoesAnyAdminExistAsync()
        => await _adminRepository.DoesAnyAdminExistAsync();

    public async Task<Result> UpdatePasswordAsync(
        Guid adminId,
        string newPassword)
    {
        var admin = await _adminRepository.GetAdminByIdAsync(adminId);

        if (!admin.IsSuccess)
            return Result.Error("Admin not found");

        var passwordHash = _hashingService.Hash(newPassword, admin.Value!.Salt);

        return await _adminRepository.UpdatePasswordAsync(
            adminId,
            passwordHash);
    }
}
