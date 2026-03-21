using System.Collections.Immutable;

namespace ScreenshotWorker;

public static class Constants
{
    public static readonly ImmutableHashSet<string> BlockedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Browser security / fingerprint
            "Sec-Fetch-Dest", "Sec-Fetch-Mode", "Sec-Fetch-Site", "Sec-Fetch-User",
            "Sec-CH-UA", "Sec-CH-UA-Mobile", "Sec-CH-UA-Platform", "Sec-CH-UA-Model",
            "Sec-CH-UA-Full-Version", "Sec-CH-UA-Arch", "Sec-CH-UA-Bitness",
            "Sec-CH-UA-Full-Version-List", "Upgrade-Insecure-Requests",

            // Connection
            "Connection", "Keep-Alive", "Transfer-Encoding", "Trailer",
            "TE", "Upgrade", "Proxy-Connection",

            // Host / routing
            "Host", "Origin", "Referer",

            // Content
            "Content-Length", "Content-Type", "Content-Encoding", "Content-Transfer-Encoding",

            // Authentication / proxy
            //"Authorization", "Proxy-Authorization",

            // Caching / compression
            "Accept-Encoding", "If-Modified-Since", "If-None-Match", "If-Range",

            // Optional / internal
            "DNT", "Via", "Forwarded"
        }.ToImmutableHashSet();
}
