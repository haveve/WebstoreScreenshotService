using System.Net;
using System.Net.Sockets;

namespace Shared.Core.Validation.Rules;

public static class UrlSecurityRules
{
    public static RuleBuilder<T, string> SafeUrl<T>(
        this RuleBuilder<T, string> rule,
        string[]? additionalBlockedHosts = null)
    {
        additionalBlockedHosts ??= [];

        return rule.Add((value, result, path) =>
        {
            if (value is null)
                return;

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                result.Add(path, "must be a valid absolute URL.");
                return;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                result.Add(path, "must use HTTP or HTTPS.");
                return;
            }

            var host = uri.Host.Trim().TrimEnd('.');

            // port restriction (same as your intent)
            if (uri.Port != 80 && uri.Port != 443)
            {
                result.Add(path, "invalid port.");
                return;
            }

            // local / private checks
            if (IsLoopback(host) || IsPrivateHost(host))
            {
                result.Add(path, $"blocked local/private URL: {value}");
                return;
            }

            // custom blocked hosts
            foreach (var blocked in additionalBlockedHosts)
            {
                if (host.Equals(blocked, StringComparison.OrdinalIgnoreCase) ||
                    host.EndsWith("." + blocked, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(path, $"blocked host: {value}");
                    return;
                }
            }
        });
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

        return host.EndsWith(".local", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPrivateIp(IPAddress ip)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var b = ip.GetAddressBytes();
            return
                b[0] == 10 ||
                (b[0] == 172 && b[1] >= 16 && b[1] <= 31) ||
                (b[0] == 192 && b[1] == 168) ||
                IPAddress.IsLoopback(ip);
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return ip.IsIPv6LinkLocal ||
                   ip.IsIPv6SiteLocal ||
                   IPAddress.IsLoopback(ip);
        }

        return false;
    }
}