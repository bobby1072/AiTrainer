using AiTrainer.Web.Domain.Models.ApiModels.Request;
using FluentValidation;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request.Validators;

public class SimilaritySearchInputValidator : AbstractValidator<SimilaritySearchInput>
{
    public SimilaritySearchInputValidator()
    {
        RuleFor(x => x.DocumentsToReturn).Must(x => x <= 20).WithMessage("You cannot return that many documents");
        RuleFor(x => x.Question).Must(x => x.Length <= 500).WithMessage("Your question is too long");
    }
}