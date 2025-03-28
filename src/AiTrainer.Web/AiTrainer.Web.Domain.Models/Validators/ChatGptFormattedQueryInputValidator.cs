using FluentValidation;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request.Validators;

public class ChatGptFormattedQueryInputValidator: AbstractValidator<ChatGptFormattedQueryInput>
{
    public ChatGptFormattedQueryInputValidator()
    {
        RuleFor(x => x.Question).NotEmpty().WithMessage("Question cannot be empty");
        RuleFor(x => x.Question).Must(x => x.Length < ).WithMessage("Question cannot be empty");
        RuleFor(x => x.ChunkId).NotEmpty().WithMessage("ChunkId cannot be empty"); }
}