using System.Security.Cryptography;

namespace WebsiteScreenshotService.Services.Security;

public class KeyService : IKeyService
{
    private const int KeySizeBytes = 32;

    public string GenerateBase64Key()
    {
        var key = new byte[KeySizeBytes];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        return Convert.ToBase64String(key);
    }

    public byte[] FromBase64(string base64Key)
    {
        if (string.IsNullOrWhiteSpace(base64Key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(base64Key));

        try
        {
            var keyBytes = Convert.FromBase64String(base64Key);

            if (keyBytes.Length != KeySizeBytes)
                throw new ArgumentException($"Invalid key length. Expected {KeySizeBytes} bytes.", nameof(base64Key));

            return keyBytes;
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid Base64 string.", nameof(base64Key), ex);
        }
    }

    public string ToBase64(byte[] keyBytes)
    {
        ArgumentNullException.ThrowIfNull(keyBytes, nameof(keyBytes));

        if (keyBytes.Length != KeySizeBytes)
            throw new ArgumentException($"Invalid key length. Expected {KeySizeBytes} bytes.", nameof(keyBytes));

        return Convert.ToBase64String(keyBytes);
    }
}