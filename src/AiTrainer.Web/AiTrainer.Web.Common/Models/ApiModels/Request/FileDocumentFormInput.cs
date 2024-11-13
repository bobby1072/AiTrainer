
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record FileDocumentFormInput
    {
        public required Guid CollectionId { get; init; }
        public required int FileType { get; init; }
        public required string FileName { get; init; }
        public required IFormFile FormFile { get; init; }
        public required DateTime DateCreated { get; init; }
    }
}
