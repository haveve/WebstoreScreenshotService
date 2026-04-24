using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Validation.Attributes;

public class RegularExpressionWithTimeoutAttribute : RegularExpressionAttribute
{
    private const int defaultTimeoutMilliseconds = 100;

    public RegularExpressionWithTimeoutAttribute(string pattern)
        : base(pattern)
    {
        MatchTimeoutInMilliseconds = defaultTimeoutMilliseconds;
    }

    public RegularExpressionWithTimeoutAttribute(string pattern, string errorMessage)
        : base(pattern)
    {
        MatchTimeoutInMilliseconds = defaultTimeoutMilliseconds;
        ErrorMessage = errorMessage;
    }
}
