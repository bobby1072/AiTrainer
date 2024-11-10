
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage(ValidatorConstants.InvalidEmail);
            RuleFor(x => x.DateCreated).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateModified).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
        }
    }
}
