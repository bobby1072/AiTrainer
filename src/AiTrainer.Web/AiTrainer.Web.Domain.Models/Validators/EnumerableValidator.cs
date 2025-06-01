using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal sealed class EnumerableValidator<T> : AbstractValidator<IEnumerable<T>>
{
    public EnumerableValidator(IValidator<T> itemValidator)
    {
        RuleForEach(x => x).SetValidator(itemValidator);
    }
}