using AiTrainer.Web.Domain.Models.ApiModels.Request;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal sealed class PotentialDocumentEditChatQueryInputValidator: BaseValidator<PotentialDocumentEditChatRawQueryInput>
{
    public PotentialDocumentEditChatQueryInputValidator()
    {
        RuleFor(x => x.ChangeRequest)
            .NotEmpty()
            .WithMessage("Change request cannot be empty");
        RuleFor(x => x.FileDocumentId)
            .NotEmpty()
            .WithMessage("File document Id cannot be empty");
    }
}