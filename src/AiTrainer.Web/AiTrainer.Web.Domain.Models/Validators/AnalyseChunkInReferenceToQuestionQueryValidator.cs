using AiTrainer.Web.Domain.Models.ApiModels.Request;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal sealed class AnalyseChunkInReferenceToQuestionQueryValidator: BaseValidator<AnalyseChunkInReferenceToQuestionQueryInput>
{
    public AnalyseChunkInReferenceToQuestionQueryValidator()
    {
        RuleFor(x => x.ChunkId).NotEmpty().WithMessage("ChunkId cannot be empty");
        RuleFor(x => x.Question).NotEmpty().WithMessage("Question cannot be empty");
        RuleFor(x => x.Question).Must(x => NotLongerThan(x, 10000)).WithMessage("Question cannot be empty");
    }
}