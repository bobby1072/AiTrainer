using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal class FileCollectionValidator : BaseValidator<FileCollection>
    {
        public FileCollectionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(ValidatorConstants.InvalidUserId);
            RuleFor(x => x.CollectionName)
                .Must(IsStringNotNullEmptyOrWhitespace)
                .WithMessage(ValidatorConstants.InvalidName);
            RuleFor(x => x.CollectionName)
                .Must(x => NotLongerThan(x, 100))
                .WithMessage(ValidatorConstants.InvalidName);
            RuleFor(x => x.DateCreated).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateCreated)
                .Must(IsDateNowOrInThePast)
                .WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified)
                .Must(IsDateNowOrInThePast)
                .WithMessage(ValidatorConstants.InvalidDate);
        }
    }
}
