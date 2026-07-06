using System.Security.Cryptography;
using System.Text;
using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Services.Security;

public class Pbkdf2HashingService : IHashingService
{
    private readonly string _defaultSalt;
    private readonly int _iterations;
    private readonly int _keySize;
    private readonly HashAlgorithmName _hashAlgorithm;

    private const string Version = "v1";
    private const char Separator = '^';

    public Pbkdf2HashingService(HashingConfigurations hashingConfigurations)
    {
        _iterations = hashingConfigurations.Iterations;
        _keySize = 64;
        _hashAlgorithm = HashAlgorithmName.SHA256;
        _defaultSalt = hashingConfigurations.DefaultSalt;
    }

    public string Hash(string input)
        => Hash(input, _defaultSalt);

    public bool Verify(string input, string expectedHash)
        => Verify(input, _defaultSalt, expectedHash);

    public string Hash(string input, string salt)
    {
        var hash = CalculateHashOnly(input, salt);
        return $"{Version}{Separator}{hash}";
    }

    public bool Verify(string input, string salt, string expectedHash)
    {
        var extractedHash = ExtractHash(expectedHash);

        var computed = CalculateHashOnly(input, salt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computed),
            Convert.FromBase64String(extractedHash));
    }

    private string CalculateHashOnly(string input, string salt)
    {
        ArgumentNullException.ThrowIfNull(salt, nameof(salt));

        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty");

        var inputArray = Encoding.UTF8.GetBytes(input);
        var saltArray = Encoding.UTF8.GetBytes(salt);

        if (saltArray.Length < 16)
            throw new ArgumentException("Salt must be at least 16 bytes");

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: inputArray,
            salt: saltArray,
            iterations: _iterations,
            hashAlgorithm: _hashAlgorithm,
            outputLength: _keySize);

        return Convert.ToBase64String(hash);
    }

    private static string ExtractHash(string hash)
    {
        var split = hash.Split(Separator, StringSplitOptions.TrimEntries);

        if (split.Length != 2)
            throw new FormatException("Invalid hash format (missing version).");

        var version = split[0];
        var extractedHash = split[1];

        if (version != Version)
            throw new NotSupportedException($"Unsupported hash version: {version}");

        return extractedHash;
    }
}
