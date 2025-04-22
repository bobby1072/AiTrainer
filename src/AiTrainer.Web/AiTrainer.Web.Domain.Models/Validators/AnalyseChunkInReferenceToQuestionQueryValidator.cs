using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators;

internal class AnalyseChunkInReferenceToQuestionQueryValidator: BaseValidator<AnalyseChunkInReferenceToQuestionQuery>
{
    public AnalyseChunkInReferenceToQuestionQueryValidator()
    {
        RuleFor(x => x.Question).NotEmpty().WithMessage("Question cannot be empty");
        RuleFor(x => x.Question).Must(x => NotLongerThan(x, 10000)).WithMessage("Question cannot be empty");
    }
}