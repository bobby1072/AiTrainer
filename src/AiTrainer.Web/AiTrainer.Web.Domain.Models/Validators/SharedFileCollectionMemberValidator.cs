using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal class SharedFileCollectionMemberValidator: BaseValidator<SharedFileCollectionMember>
{
    public SharedFileCollectionMemberValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required");
        RuleFor(x => x.CollectionId).NotEmpty().WithMessage("Collection id is required");
    }
}