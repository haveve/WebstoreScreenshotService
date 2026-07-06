using System.Security.Cryptography;

namespace WebsiteScreenshotService.Services;

public class TotpService
{
    private const int TimestepSeconds = 30;
    private const int TotpSize = 6;
    private const int DriftSteps = 0;        
    private const string Algorithm = "SHA1";

    public string GenerateSecret()
    {
        var bytes = new byte[20]; // 160-bit secret
        RandomNumberGenerator.Fill(bytes);
        return Base32Encode(bytes);
    }

    public string GenerateToken(string base32Secret)
    {
        var secret = Base32Decode(base32Secret);
        var timestep = GetCurrentTimeStepNumber();

        return GenerateTotp(secret, timestep);
    }

    public bool ValidateToken(string base32Secret, string token)
    {
        var secret = Base32Decode(base32Secret);
        var currentStep = GetCurrentTimeStepNumber();

        for (int i = -DriftSteps; i <= DriftSteps; i++)
        {
            var computed = GenerateTotp(secret, currentStep + i);
            if (computed == token)
                return true;
        }

        return false;
    }

    public string GenerateOtpAuthUrl(string account, string issuer, string base32Secret)
    {
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}" +
               $"?secret={base32Secret}" +
               $"&issuer={Uri.EscapeDataString(issuer)}" +
               $"&digits={TotpSize}" +
               $"&algorithm={Algorithm}" +
               $"&period={TimestepSeconds}";
    }

    #region internal helpers

    private static long GetCurrentTimeStepNumber()
    {
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return unixTime / TimestepSeconds;
    }

    private static string GenerateTotp(byte[] key, long timestep)
    {
        var timestepBytes = BitConverter.GetBytes(timestep);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(timestepBytes);

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(timestepBytes);

        // Dynamic truncation (RFC 4226)
        int offset = hash[^1] & 0x0F;

        int binary =
            ((hash[offset] & 0x7f) << 24) |
            (hash[offset + 1] << 16) |
            (hash[offset + 2] << 8) |
            (hash[offset + 3]);

        int otp = binary % (int)Math.Pow(10, TotpSize);
        return otp.ToString(new string('0', TotpSize));
    }

    // -------- Base32 (simple implementation) --------

    private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    private static string Base32Encode(byte[] data)
    {
        int i = 0, index = 0;
        var result = string.Empty;

        while (i < data.Length)
        {
            int currByte = data[i] >= 0 ? data[i] : (data[i] + 256);

            if (index > 3)
            {
                int nextByte = (i + 1) < data.Length
                    ? (data[i + 1] >= 0 ? data[i + 1] : (data[i + 1] + 256))
                    : 0;

                int digit = currByte & (0xFF >> index);
                index = (index + 5) % 8;
                digit <<= index;
                digit |= nextByte >> (8 - index);
                result += Base32Chars[digit];
                i++;
            }
            else
            {
                int digit = (currByte >> (8 - (index + 5))) & 0x1F;
                index = (index + 5) % 8;
                if (index == 0) i++;
                result += Base32Chars[digit];
            }
        }

        return result;
    }

    private static byte[] Base32Decode(string input)
    {
        input = input.TrimEnd('=').ToUpperInvariant();

        var output = new byte[input.Length * 5 / 8];
        int bitIndex = 0, byteIndex = 0;

        foreach (var c in input)
        {
            int val = Base32Chars.IndexOf(c);
            if (val < 0) throw new ArgumentException("Invalid Base32 character.");

            for (int i = 4; i >= 0; i--)
            {
                if ((val & (1 << i)) != 0)
                    output[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }
        }

        return output;
    }
    #endregion
}