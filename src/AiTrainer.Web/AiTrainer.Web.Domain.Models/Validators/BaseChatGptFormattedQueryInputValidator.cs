using AiTrainer.Web.Domain.Models.ApiModels.Request;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal sealed class BaseChatGptFormattedQueryInputValidator : BaseValidator<BaseChatGptFormattedQueryInput>
{
    private static readonly Type DefinedQueryFormatsEnumType = typeof(DefinedQueryFormatsEnum);

    public BaseChatGptFormattedQueryInputValidator()
    {
        RuleFor(x => x.DefinedQueryFormatsEnum)
            .Must(x => Enum.IsDefined(DefinedQueryFormatsEnumType, x))
            .WithMessage("DefinedQueryFormatsEnum is not a valid value");
    }
}
