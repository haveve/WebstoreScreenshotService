using System.Security.Cryptography;
using System.Text;

namespace WebsiteScreenshotService.Services.Security;

public sealed class AesEncryptionService : IEncryptionService
{
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;
    private const string Version = "v1";

    public string Encrypt(
        string plaintext,
        byte[] key,
        EncryptionContext context)
    {
        ArgumentNullException.ThrowIfNull(plaintext);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(context);

        var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSizeBytes];

        var aad = BuildAad(context);

        using var aes = new AesGcm(key, TagSizeBytes);

        aes.Encrypt(
            nonce,
            plaintextBytes,
            ciphertext,
            tag,
            aad);

        var result = new byte[
            NonceSizeBytes +
            TagSizeBytes +
            ciphertext.Length];

        var offset = 0;

        nonce.CopyTo(result.AsSpan(offset));
        offset += NonceSizeBytes;

        tag.CopyTo(result.AsSpan(offset));
        offset += TagSizeBytes;

        ciphertext.CopyTo(result.AsSpan(offset));

        return Convert.ToBase64String(result);
    }

    public string Decrypt(
        string encrypted,
        byte[] key,
        EncryptionContext context)
    {
        ArgumentNullException.ThrowIfNull(encrypted);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(context);

        var full = Convert.FromBase64String(encrypted);

        if (full.Length < NonceSizeBytes + TagSizeBytes)
            throw new CryptographicException("Invalid encrypted payload.");

        var nonce = full.AsSpan(0, NonceSizeBytes);

        var tag = full.AsSpan(
            NonceSizeBytes,
            TagSizeBytes);

        var ciphertext = full.AsSpan(
            NonceSizeBytes + TagSizeBytes);

        var plaintext = new byte[ciphertext.Length];

        var aad = BuildAad(context);

        using var aes = new AesGcm(key, TagSizeBytes);

        try
        {
            aes.Decrypt(
                nonce,
                ciphertext,
                tag,
                plaintext,
                aad);
        }
        catch (CryptographicException)
        {
            throw new CryptographicException("Authentication failed. Ciphertext or AAD may have been tampered with.");
        }

        return Encoding.UTF8.GetString(plaintext);
    }

    private static byte[] BuildAad(EncryptionContext context)
    {
        var aadString = $"version={Version}|tenant={context.TenantId}|purpose={context.Purpose}";
        return Encoding.UTF8.GetBytes(aadString);
    }
}