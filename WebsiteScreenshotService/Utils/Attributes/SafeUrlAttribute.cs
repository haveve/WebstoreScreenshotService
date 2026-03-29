using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;

namespace WebsiteScreenshotService.Utils.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SafeUrlAttribute : ValidationAttribute
{
    /// <summary>
    /// Optionally allow custom additional hosts/IPs to block
    /// </summary>
    public string[] AdditionalBlockedHosts { get; set; } = Array.Empty<string>();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is not string urlString || string.IsNullOrWhiteSpace(urlString))
            return new ValidationResult($"{validationContext.MemberName} must be a valid URL.");

        if (!Uri.TryCreate(urlString, UriKind.Absolute, out var uri))
            return new ValidationResult($"{validationContext.MemberName} is not a valid absolute URL.");

        // Only HTTP/HTTPS allowed
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return new ValidationResult($"{validationContext.MemberName} must use HTTP or HTTPS.");

        var host = uri.Host;

        // Block immediate loopback or private hosts
        if (IsLoopback(host) || IsPrivateHost(host))
            return new ValidationResult($"Access to private or local URLs is not allowed: {urlString}");

        // Block custom hosts
        foreach (var blocked in AdditionalBlockedHosts)
        {
            if (string.Equals(host, blocked, StringComparison.OrdinalIgnoreCase))
                return new ValidationResult($"Access to blocked host is not allowed: {urlString}");
        }

        // DNS resolution check
        if (!IsDnsSafe(host).GetAwaiter().GetResult())
            return new ValidationResult($"Access to URL is blocked because it resolves to private network: {urlString}");

        return ValidationResult.Success;
    }

    private static bool IsLoopback(string host)
    {
        if (IPAddress.TryParse(host, out var ip))
            return IPAddress.IsLoopback(ip);

        return host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPrivateHost(string host)
    {
        if (IPAddress.TryParse(host, out var ip))
            return IsPrivateIp(ip);

        // Block .local and other internal-looking hostnames
        return host.EndsWith(".local", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPrivateIp(IPAddress ip)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
        {
            var bytes = ip.GetAddressBytes();
            return
                bytes[0] == 10 ||
                (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                (bytes[0] == 192 && bytes[1] == 168) ||
                ip.Equals(IPAddress.Loopback) ||
                ip.Equals(IPAddress.Any);
        }
        else if (ip.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
        {
            return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || IPAddress.IsLoopback(ip);
        }

        return false;
    }

    private static async Task<bool> IsDnsSafe(string host)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host);
            foreach (var ip in addresses)
            {
                if (IsPrivateIp(ip))
                    return false;
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
