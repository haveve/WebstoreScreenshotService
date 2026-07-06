using System.Text.RegularExpressions;

namespace Shared.Core.Validation;

public static class RegexPatterns
{
    public static readonly Regex NickName =
        new(@"^[a-zA-Z_$-]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public static readonly Regex Locale =
        new(@"^[a-z]{2}-[A-Z]{2}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public static readonly Regex Timezone =
        new(@"^[A-Za-z]+\/[A-Za-z_]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public static readonly Regex Header =
        new(@"^[A-Za-z0-9\-]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public static readonly Regex Email =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(100));
}
