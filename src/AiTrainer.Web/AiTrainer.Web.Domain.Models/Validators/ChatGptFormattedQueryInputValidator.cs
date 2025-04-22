using AiTrainer.Web.Domain.Models.ApiModels.Request;
using BT.Common.Helpers.Extensions;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal class ChatGptFormattedQueryInputValidator : BaseValidator<ChatGptFormattedQueryInput>
{
    private static readonly Type DefinedQueryFormatsEnumType = typeof(DefinedQueryFormatsEnum);
    public ChatGptFormattedQueryInputValidator()
    {
        RuleFor(x => x.InputJson).Must(x => x.IsValidJson()).WithMessage("Input JSON is invalid");
        RuleFor(x => x.ChunkId).NotEmpty().WithMessage("ChunkId cannot be empty");
        RuleFor(x => x.DefinedQueryFormatsEnum).Must(x => Enum.IsDefined(DefinedQueryFormatsEnumType, x)).WithMessage("DefinedQueryFormatsEnum is not a valid value");
    }
}