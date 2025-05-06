
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal sealed class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage(ValidatorConstants.InvalidEmail);
            RuleFor(x => x.DateCreated).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateCreated).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
        }
    }
}
