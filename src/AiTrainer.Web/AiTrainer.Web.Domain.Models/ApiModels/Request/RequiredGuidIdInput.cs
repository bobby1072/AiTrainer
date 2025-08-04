using System.ComponentModel.DataAnnotations;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public sealed record RequiredGuidIdInput
    {
        [Required]
        public required Guid Id { get; init; }
    }
}
