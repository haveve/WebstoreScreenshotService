using System.Security.Cryptography;
using System.Text;

namespace WebsiteScreenshotService.Services.Security;

public class AesEncryptionService : IEncryptionService
{
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    public string Encrypt(string input, byte[] key)
    {
        var plaintext = Encoding.UTF8.GetBytes(input);

        var nonce = new byte[NonceSizeBytes];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSizeBytes];

        using var aes = new AesGcm(key, TagSizeBytes);

        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var result = new byte[NonceSizeBytes + TagSizeBytes + ciphertext.Length];

        var offset = 0;

        nonce.CopyTo(result.AsSpan(offset));
        offset += NonceSizeBytes;

        tag.CopyTo(result.AsSpan(offset));
        offset += TagSizeBytes;

        ciphertext.CopyTo(result.AsSpan(offset));

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string input, byte[] key)
    {
        var full = Convert.FromBase64String(input);

        var nonce = full.AsSpan(0, NonceSizeBytes);
        var tag = full.AsSpan(NonceSizeBytes, TagSizeBytes);
        var ciphertext = full.AsSpan(NonceSizeBytes + TagSizeBytes);

        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSizeBytes);

        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }
}
