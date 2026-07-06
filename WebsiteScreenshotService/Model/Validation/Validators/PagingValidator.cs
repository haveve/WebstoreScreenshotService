using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace WebsiteScreenshotService.Model.Validation.Validators;

public sealed class PagingValidator : Validator<Paging>
{
    public PagingValidator()
    {
        RuleFor(nameof(Paging.Page), x => x.Page)
            .Range(1, 10_000, "Page must be between 1 and 10,000.");

        RuleFor(nameof(Paging.PageSize), x => x.PageSize)
            .Range(1, 200, "PageSize must be between 1 and 200.");

        RuleFor(nameof(Paging.Query), x => x.Query)
            .Must(v => v is null || v.Trim().Length > 0, "Query cannot be empty or whitespace.")
            .Must(v => v is null || v.Length <= 150, "Query must not exceed 150 characters.");

        RuleFor(nameof(Paging.SearchScope), x => x.SearchScope)
            .Must(v => Enum.IsDefined(v),
                $"Invalid SearchScope value.");

        RuleFor(nameof(Paging.CategoryIds), x => x.CategoryIds)
            .Must(v => v is null || v.Length <= 100, "CategoryIds cannot contain more than 100 items.");
    }
}
