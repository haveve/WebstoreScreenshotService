using Shared.Core.Validation;
using Shared.Core.Validation.Rules;

namespace Shared.Core.Contracts.ScreeshotModel.Validation;

public sealed class MakeScreenshotModelValidator : Validator<MakeScreenshotModel>
{
    public MakeScreenshotModelValidator()
    {
        Include(nameof(MakeScreenshotModel.ScreenshotOptionsModel), 
            msm => msm.ScreenshotOptionsModel, 
            new ScreenshotOptionsValidator());

        Include(nameof(MakeScreenshotModel.UserInformation),
            msm => msm.UserInformation,
            new UserInformationValidator());

        RuleFor(nameof(MakeScreenshotModel.ScreenshotId), msm => msm.ScreenshotId)
            .Required("");

        RuleFor(nameof(MakeScreenshotModel.ConfirmationToken), msm => msm.ConfirmationToken)
            .Required("");
    }
}