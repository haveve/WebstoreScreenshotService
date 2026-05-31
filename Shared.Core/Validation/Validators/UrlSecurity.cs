using System.Net;
using System.Net.Sockets;

namespace Shared.Core.Validation.Validators;

public class UrlSecurity
{
    public static async Task<bool> IsIpBlockedWithDnsLookup(string? value, string[] additionalBlockedHosts, IPAddress[] additionalBlockedIps)
    {
        if (IsIpBlocked(value, additionalBlockedHosts, out string? host) || host is null)
            return true;

        IPAddress[]? addresses;

        try
        {
            addresses = Dns.GetHostAddresses(host);
        }
        catch
        {
            return true;
        }

        if (addresses is null || addresses.Length == 0)
        {
            return true;
        }

        foreach (var address in addresses)
        {
            if (IsBlockedIp(address))
            {
                return true;
            }

            if (additionalBlockedIps.Contains(address))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsIpBlocked(string? value, string[] additionalBlockedHosts, out string? host)
    {
        host = null;

        if (value is null)
            return false;

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return true;

        if (uri.Scheme is not ("http" or "https"))
        {
            return true;
        }

        host = uri.Host.Trim().TrimEnd('.');

        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!HasAllowedPort(uri))
        {
            return true;
        }

        if (IPAddress.TryParse(host, out var ip) && IsBlockedIp(ip))
        {
            return true;
        }

        foreach (var blocked in additionalBlockedHosts)
        {
            if (host.Equals(blocked, StringComparison.OrdinalIgnoreCase)
                || host.EndsWith("." + blocked, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAllowedPort(Uri uri)
    {
        return uri switch
        {
            { Scheme: "http", IsDefaultPort: true } => true,
            { Scheme: "https", IsDefaultPort: true } => true,

            { Scheme: "http", Port: 80 } => true,
            { Scheme: "https", Port: 443 } => true,

            _ => false
        };
    }

    private static bool IsBlockedIp(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
            return true;

        if (ip.AddressFamily == AddressFamily.InterNetwork)
            return IsBlockedIpv4(ip);

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            return IsBlockedIpv6(ip);

        return true;
    }

    private static bool IsBlockedIpv4(IPAddress ip)
    {
        var b = ip.GetAddressBytes();

        // 0.0.0.0/8
        if (b[0] == 0)
            return true;

        // 10.0.0.0/8
        if (b[0] == 10)
            return true;

        // 100.64.0.0/10
        if (b[0] == 100 && b[1] >= 64 && b[1] <= 127)
            return true;

        // 127.0.0.0/8
        if (b[0] == 127)
            return true;

        // 169.254.0.0/16
        if (b[0] == 169 && b[1] == 254)
            return true;

        // 172.16.0.0/12
        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
            return true;

        // 192.168.0.0/16
        if (b[0] == 192 && b[1] == 168)
            return true;

        // multicast 224.0.0.0/4
        if (b[0] >= 224 && b[0] <= 239)
            return true;

        // reserved 240.0.0.0/4
        if (b[0] >= 240)
            return true;

        return false;
    }

    private static bool IsBlockedIpv6(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
            return true;

        if (ip.IsIPv6LinkLocal)
            return true;

        if (ip.IsIPv6Multicast)
            return true;

        var bytes = ip.GetAddressBytes();

        // fc00::/7
        if ((bytes[0] & 0xfe) == 0xfc)
            return true;

        return false;
    }
}
