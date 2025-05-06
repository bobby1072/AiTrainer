using AiTrainer.Web.Domain.Models.ApiModels.Request;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal sealed class ChatGptFormattedQueryInputValidator : BaseValidator<ChatGptFormattedQueryInput>
{
    private static readonly Type DefinedQueryFormatsEnumType = typeof(DefinedQueryFormatsEnum);

    public ChatGptFormattedQueryInputValidator()
    {
        RuleFor(x => x.DefinedQueryFormatsEnum)
            .Must(x => Enum.IsDefined(DefinedQueryFormatsEnumType, x))
            .WithMessage("DefinedQueryFormatsEnum is not a valid value");
    }
}
