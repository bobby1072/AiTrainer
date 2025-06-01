using AiTrainer.Web.Domain.Models.Validators;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Extensions;

public static class ValidatorExtensions
{
    public static IValidator<IEnumerable<T>> CreateEnumerableValidator<T>(this IValidator<T> itemValidator) => new EnumerableValidator<T>(itemValidator);
}