using BT.Common.FastArray.Proto;
using FluentValidation.Results;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class ValidatorExtensions
    {
        public static string ToErrorString(this IEnumerable<ValidationFailure> errorResults)
        {
            return string.Join(". ", errorResults.FastArraySelect(x => x.ErrorMessage));
        }
    }
}