
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record FileDocumentFormInput
    {
        public Guid? CollectionId { get; init; }
        public required IFormFile FormFile { get; init; }
    }
}
