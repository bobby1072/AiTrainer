
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage(ValidatorConstants.InvalidEmail);
            RuleFor(x => x.Username).Must(IsStringNotNullEmptyOrWhitespace).WithMessage(ValidatorConstants.InvalidName);
            RuleFor(x => x.Username).Must(x => NotLongerThan(x, 100)).WithMessage(ValidatorConstants.InvalidName);
            RuleFor(x => x.DateCreated).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
        }
    }
}
