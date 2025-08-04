using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public sealed record FileDocumentSaveFormInput
    {
        public Guid? CollectionId { get; init; }

        public required IFormFile FileToCreate { get; init; }

        public string? FileDescription { get; init; }
    }
}
