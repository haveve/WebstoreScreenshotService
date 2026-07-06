using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace Shared.Core.Contracts.ScreeshotModel.Validation;

public sealed class UserInformationValidator : Validator<UserInformation>
{
    public UserInformationValidator()
    {
        RuleFor(nameof(UserInformation.UserId), u => u.UserId)
            .Required("");
    }
}